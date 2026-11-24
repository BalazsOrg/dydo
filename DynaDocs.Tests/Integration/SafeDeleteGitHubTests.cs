namespace DynaDocs.Tests.Integration;

using DynaDocs.Services;
using System.Diagnostics;
using System.Text.Json.Nodes;

[Collection("Integration")]
public sealed class SafeDeleteGitHubTests
{
    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (a proved closed-world state passes the remote eligibility check)")]
    [Theory]
    [InlineData("public",false,0)]
    [InlineData("public",true,12)]
    [InlineData("private",false,0)]
    public void PublicExactRefWithoutHeadsOrBases_IsEligible(string visibility, bool fork, int forks)
    {
        using var fixture = new SafeDeleteTests();
        var target = Target(fixture);
        Assert.Null(Check(fixture,target,start => Response(start,target,visibility,fork,forks)));
        Assert.Equal("R_selected",target.Metadata?.Id);
    }

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (on a proved closed-world state the remote ref is deleted with the archived worktree)")]
    [Fact]
    public void PrivateClosedWorld_RealCliAndBareRemoteArchiveAndRestore() => Journey("private",false,0);

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (on a proved closed-world state the remote ref is deleted with the archived worktree)")]
    [Fact]
    public void PublicFork_LeasedOwnDeletionAuthorizesRemainingLocalSteps() => Journey("public",true,12);

    internal static void Journey(string visibility, bool fork, int forks)
    {
        using var fixture = new SafeDeleteTests(); fixture.AddRemote(); fixture.AddTree();
        File.WriteAllText(Path.Combine(fixture.Tree,"unique.txt"),"unique working bytes\n");
        fixture.Dependencies.CheckGitHub = target => Check(fixture,target,start => Response(start,target,visibility,fork,forks));
        Assert.True(fixture.Run("worktree",fixture.Tree) == 0,fixture.Output);
        Assert.False(Directory.Exists(fixture.Tree));
        Assert.False(fixture.Has("refs/heads/topic"));
        Assert.Empty(SafeDeleteTests.Git(fixture.Remote,"for-each-ref","refs/heads/topic"));
        Assert.Equal(0,fixture.Run("restore",fixture.Id));
        Assert.Equal("unique working bytes\n",File.ReadAllText(Path.Combine(fixture.Tree,"unique.txt")));
        Assert.Contains("R_selected",File.ReadAllText(Path.Combine(fixture.Entry,"manifest.json")));
        Assert.Empty(SafeDeleteTests.Git(fixture.Remote,"for-each-ref","refs/heads/topic"));
    }

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (remote cleanup only on proved closed-world state)")]
    [Theory]
    [InlineData("upstream/repo")]
    [InlineData("other-fork/repo")]
    public void ForkBranchUsedAsUpstreamPullRequestHead_RefusesWithRemoteLocalAndFilesIntact(string destination)
    {
        Refusal((start,target) =>
        {
            var response = Response(start,target,"public",true,2);
            if (IsHeads(start)) SetNodes(response,"heads",Pull("R_selected","owner/repo","topic","R_destination",destination,"main"));
            return response;
        },"open PR head");
    }

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (remote cleanup only on proved closed-world state)")]
    [Theory]
    [InlineData("head")]
    [InlineData("base")]
    public void PrivateClosedWorld_ExactOwnHeadOrBaseIsProtected(string side)
    {
        Refusal((start,target) =>
        {
            var response = Response(start,target,"private");
            if (IsOwn(start)) SetNodes(response,"own",Pull("R_selected","owner/repo",side == "head" ? "topic" : "other","R_selected","owner/repo",side == "base" ? "topic" : "main"));
            return response;
        },"open PR " + side);
    }

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (remote cleanup only on proved closed-world state)")]
    [Theory]
    [InlineData("private fork","closed-world")]
    [InlineData("private forks","closed-world")]
    [InlineData("internal","internal")]
    [InlineData("private mismatch","inconsistent")]
    [InlineData("missing forks","unavailable")]
    [InlineData("metadata repo","identity")]
    [InlineData("metadata id","identity")]
    [InlineData("negative forks","inconsistent")]
    [InlineData("fractional forks","unavailable")]
    [InlineData("visibility","unknown")]
    [InlineData("errors","GraphQL")]
    [InlineData("errors malformed","GraphQL")]
    [InlineData("data null","unavailable")]
    [InlineData("repository null","unavailable")]
    [InlineData("ref null","unproven")]
    [InlineData("ref branch","identity/OID")]
    [InlineData("ref prefix","identity/OID")]
    [InlineData("ref oid","identity/OID")]
    [InlineData("ref repository","identity")]
    [InlineData("ref id","incomplete")]
    [InlineData("repo name","identity")]
    [InlineData("repo id","identity")]
    [InlineData("pr null","unavailable")]
    [InlineData("pr state","state")]
    [InlineData("pr head null","unavailable")]
    [InlineData("pr base null","unavailable")]
    [InlineData("pr id","incomplete")]
    [InlineData("pr branch","inconsistent")]
    [InlineData("pr head identity","identity")]
    [InlineData("own base identity","identity")]
    [InlineData("own owner identity","identity")]
    [InlineData("count missing","unavailable")]
    [InlineData("count negative","count")]
    [InlineData("count fractional","unavailable")]
    [InlineData("count short","count")]
    [InlineData("nodes null","nodes")]
    [InlineData("cursor missing","unavailable")]
    [InlineData("cursor null","cursor")]
    [InlineData("cursor wrong type","cursor")]
    [InlineData("empty nonterminal","cursor/count")]
    public void InvalidAuthorityOrResponse_RefusesIntact(string defect, string reason)
        => Refusal((start,target) => Damage(Response(start,target),start,defect),reason);

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (remote cleanup only on proved closed-world state)")]
    [Theory]
    [InlineData("fork")]
    [InlineData("forks")]
    [InlineData("identity")]
    [InlineData("PR")]
    [InlineData("OID")]
    public void FinalPrePushChange_RefusesBeforeRemoteDeletion(string change)
    {
        using var fixture = new SafeDeleteTests(); fixture.AddRemote(); fixture.AddTree();
        var changed = false;
        fixture.Dependencies.Checkpoint = point => { if (point == "before-remote-delete") changed = true; };
        fixture.Dependencies.CheckGitHub = target => Check(fixture,target,start =>
        {
            var response = Response(start,target,"private",changed && change == "fork",changed && change == "forks" ? 1 : 0);
            if (changed && change == "identity" && !IsGraph(start)) response["node_id"] = "R_replaced";
            if (changed && change == "OID" && IsGraph(start)) response["data"]!["repository"]!["ref"]!["target"]!["oid"] = new string('b',40);
            if (changed && change == "PR" && IsHeads(start)) SetNodes(response,"heads",Pull());
            return response;
        });
        Assert.Equal(2,fixture.Run("worktree",fixture.Tree));
        Intact(fixture);
        Assert.True(Directory.Exists(fixture.Entry));
    }

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (remote cleanup only on proved closed-world state)")]
    [Theory]
    [InlineData("fork")]
    [InlineData("forks")]
    public void FinalMetadataRead_AfterCompletePrTraversalRefusesNewForkAuthority(string change)
    {
        using var fixture = new SafeDeleteTests(); fixture.AddRemote(); fixture.AddTree();
        var metadataReads = 0;
        fixture.Dependencies.CheckGitHub = target => Check(fixture,target,start =>
        {
            if (!IsGraph(start)) metadataReads++;
            return Response(start,target,"private",metadataReads == 4 && change == "fork",metadataReads == 4 && change == "forks" ? 1 : 0);
        });
        Assert.Equal(2,fixture.Run("worktree",fixture.Tree));
        Assert.Equal(4,metadataReads);
        Intact(fixture);
        Assert.Contains("closed-world",fixture.Output);
    }

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (after the remote step, a failed recheck keeps the remaining local targets)")]
    [Theory]
    [InlineData("recreated")]
    [InlineData("API failure")]
    [InlineData("metadata")]
    [InlineData("base PR")]
    [InlineData("file change")]
    [InlineData("local ref change")]
    public void AfterOwnDeletion_RecheckFailureRetainsRemainingWorktreeAndLocalBranch(string change)
    {
        using var fixture = new SafeDeleteTests(); fixture.AddRemote(); fixture.AddTree();
        var changed = false;
        fixture.Dependencies.Checkpoint = point =>
        {
            if (point != "before-worktree-delete") return;
            changed = true;
            if (change == "recreated") SafeDeleteTests.Git(fixture.Main,"push","origin","topic");
            if (change == "file change") File.WriteAllText(Path.Combine(fixture.Tree,"file.txt"),"late bytes");
            if (change == "local ref change")
            {
                var oid = SafeDeleteTests.Git(fixture.Main,"commit-tree",SafeDeleteTests.Git(fixture.Main,"rev-parse","topic^{tree}"),"-p",SafeDeleteTests.Git(fixture.Main,"rev-parse","topic"),"-m","late");
                SafeDeleteTests.Git(fixture.Main,"update-ref","refs/heads/topic",oid);
            }
        };
        fixture.Dependencies.CheckGitHub = target => Check(fixture,target,start =>
        {
            var response = Response(start,target,"public",true,changed && change == "metadata" ? 2 : 1);
            if (changed && change == "API failure" && IsGraph(start)) response["errors"] = new JsonArray(new JsonObject { ["message"] = "denied" });
            if (changed && change == "base PR" && IsOwn(start)) SetNodes(response,"own",Pull("R_other","other/repo","foreign","R_selected","owner/repo","topic"));
            return response;
        });
        Assert.Equal(2,fixture.Run("worktree",fixture.Tree));
        Assert.True(Directory.Exists(fixture.Tree)); Assert.True(fixture.Has("refs/heads/topic"));
        Assert.Equal(change == "recreated",SafeDeleteTests.Git(fixture.Remote,"for-each-ref","refs/heads/topic").Length > 0);
        Assert.Contains("remote deleted",fixture.Output);
        Assert.Contains("\"complete\": true",File.ReadAllText(Path.Combine(fixture.Entry,"manifest.json")),StringComparison.OrdinalIgnoreCase);
    }

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (after the remote step, a failed recheck keeps the remaining local targets)")]
    [Theory]
    [InlineData("before-local-delete")]
    [InlineData("before-worktree-delete")]
    public void EveryRemainingDestructiveStep_RechecksPrEligibility(string point)
    {
        using var fixture = new SafeDeleteTests(); fixture.AddRemote();
        if (point == "before-worktree-delete") fixture.AddTree();
        var changed = false;
        fixture.Dependencies.Checkpoint = current => { if (current == point) changed = true; };
        fixture.Dependencies.CheckGitHub = target => Check(fixture,target,start =>
        {
            var response = Response(start,target,"private");
            if (changed && IsOwn(start)) SetNodes(response,"own",Pull());
            return response;
        });
        Assert.Equal(2,fixture.Run(point == "before-worktree-delete" ? ["worktree",fixture.Tree] : ["branch","topic"]));
        Assert.True(fixture.Has("refs/heads/topic"));
        Assert.Equal(point == "before-worktree-delete",Directory.Exists(fixture.Tree));
        Assert.Empty(SafeDeleteTests.Git(fixture.Remote,"for-each-ref","refs/heads/topic"));
    }

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (after the remote step, a failed recheck keeps the remaining local targets)")]
    [Fact]
    public void AfterOwnDeletion_ApiRefSurvivesWhileGitIsAbsent_RetainsVerifiedArchiveAndLocalTarget()
    {
        using var fixture = new SafeDeleteTests(); fixture.AddRemote(); fixture.AddTree();
        var tip = SafeDeleteTests.Git(fixture.Main,"rev-parse","topic");
        File.WriteAllText(Path.Combine(fixture.Tree,"unique.txt"),"receipt witness bytes\n");
        var afterLease = false;
        var receiptRefReads = 0;
        fixture.Dependencies.Checkpoint = point =>
        {
            if (point != "before-worktree-delete") return;
            Assert.Empty(SafeDeleteTests.Git(fixture.Main,"ls-remote","origin","refs/heads/topic"));
            afterLease = true;
        };
        fixture.Dependencies.CheckGitHub = target => Check(fixture,target,start =>
        {
            if (afterLease && IsGraph(start))
            {
                Assert.True(target.DeletedByInvocation);
                Assert.NotNull(target.Metadata);
                Assert.False(IsHeads(start));
                Assert.Empty(SafeDeleteTests.Git(fixture.Main,"ls-remote","origin","refs/heads/topic"));
                receiptRefReads++;
            }
            return Response(start,target,"public",true,12,apiRefPresent: true);
        });

        var exit = fixture.Run("worktree",fixture.Tree);

        Assert.True(afterLease);
        Assert.True(receiptRefReads > 0);
        Assert.Equal(2,exit);
        Assert.Contains("Remote ref recreated or own-deletion receipt is invalid",fixture.Output);
        Assert.Contains("remote deleted; worktree/local branch retained",fixture.Output);
        Assert.Contains("Independent proof passed",fixture.Output);
        Assert.Empty(SafeDeleteTests.Git(fixture.Main,"ls-remote","origin","refs/heads/topic"));
        Assert.Equal(tip,SafeDeleteTests.Git(fixture.Main,"rev-parse","topic"));
        Assert.Equal(tip,SafeDeleteTests.Git(fixture.Tree,"rev-parse","HEAD"));
        Assert.Equal("base\n",File.ReadAllText(Path.Combine(fixture.Tree,"file.txt")));
        Assert.Equal("receipt witness bytes\n",File.ReadAllText(Path.Combine(fixture.Tree,"unique.txt")));
        Assert.True(new SafeDeleteStore(fixture.ArchiveRoot,TextWriter.Null).Load(fixture.Id).Complete);
        Assert.Equal("receipt witness bytes\n",File.ReadAllText(Path.Combine(fixture.Entry,"files","unique.txt")));
    }

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (remote cleanup only on proved closed-world state)")]
    [Theory]
    [InlineData("missing metadata","own-deletion receipt is invalid")]
    [InlineData("unrelated repository","metadata changed")]
    [InlineData("without lease","authority is unproven")]
    public void OwnDeletionReceipt_MissingOrUnownedContextCannotAuthorizeApiAbsence(string context, string reason)
    {
        using var fixture = new SafeDeleteTests();
        var target = Target(fixture);
        Assert.Null(Check(fixture,target,start => Response(start,target,"public",true,12,apiRefPresent: true)));
        target.DeletedByInvocation = context != "without lease";
        if (context == "missing metadata") target.Metadata = null;
        if (context == "unrelated repository") target.Metadata = ("R_unrelated","public",true,12);

        var result = Check(fixture,target,start => Response(start,target,"public",true,12,apiRefPresent: false));

        Assert.Contains(reason,result);
    }

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (a proved closed-world state passes the remote eligibility check)")]
    [Theory]
    [InlineData("public")]
    [InlineData("private")]
    public void InitiallyAbsentClosedWorld_CompleteOwnAssociationsAuthorizeAbsence(string visibility)
    {
        using var fixture = new SafeDeleteTests();
        var target = new SafeDeleteGitHubTarget("https://github.com/owner/repo.git","topic",null);
        Assert.Null(Check(fixture,target,start => AbsentResponse(start,target,visibility)));
        Assert.Equal("R_selected",target.Metadata?.Id);
    }

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (a proved closed-world absence of the remote ref lets the local targets be deleted)")]
    [Theory]
    [InlineData("public",true)]
    [InlineData("private",true)]
    [InlineData("public",false)]
    [InlineData("private",false)]
    public void InitiallyAbsentClosedWorld_ArchivesWithoutPushAndRestoresIndependently(string visibility, bool worktree)
    {
        using var fixture = new SafeDeleteTests();
        PrepareInitiallyAbsent(fixture,visibility,worktree);
        var staged = worktree ? SafeDeleteTests.Git(fixture.Tree,"rev-parse",":file.txt") : null;
        Assert.True(fixture.Run(worktree ? ["worktree",fixture.Tree] : ["branch","topic"]) == 0,fixture.Output);
        Assert.False(fixture.Has("refs/heads/topic"));
        Assert.False(Directory.Exists(fixture.Tree));
        Assert.Contains("Independent proof passed",fixture.Output);
        SafeDeleteTests.Git(fixture.Main,"reflog","expire","--expire=now","--all");
        SafeDeleteTests.Git(fixture.Main,"gc","--prune=now");
        if (staged != null) Assert.False(GitRunner.RunIsolated(fixture.Main,new Dictionary<string,string>(),"cat-file","-e",staged).Ok);
        Assert.True(fixture.Run("restore",fixture.Id) == 0,fixture.Output);
        Assert.True(fixture.Has("refs/heads/topic"));
        if (worktree)
        {
            Assert.Equal("staged only",SafeDeleteTests.Git(fixture.Tree,"show",":file.txt"));
            Assert.Equal("working bytes\n",File.ReadAllText(Path.Combine(fixture.Tree,"file.txt")));
        }
        Assert.Empty(SafeDeleteTests.Git(fixture.Remote,"for-each-ref","refs/heads/topic"));
    }

    internal static void PrepareInitiallyAbsent(SafeDeleteTests fixture, string visibility, bool worktree)
    {
        fixture.AddRemote();
        SafeDeleteTests.Git(fixture.Remote,"update-ref","-d","refs/heads/topic");
        SafeDeleteTests.Git(fixture.Main,"config","remote.origin.receivepack","dydo-must-not-push");
        if (worktree)
        {
            fixture.AddTree();
            File.WriteAllText(Path.Combine(fixture.Tree,"file.txt"),"staged only\n");
            SafeDeleteTests.Git(fixture.Tree,"add","file.txt");
            File.WriteAllText(Path.Combine(fixture.Tree,"file.txt"),"working bytes\n");
        }
        fixture.Dependencies.Checkpoint = point =>
        {
            if (point is "before-fetch" or "before-remote-delete") throw new IOException("Initially absent ref must not fetch or push");
        };
        fixture.Dependencies.CheckGitHub = target => Check(fixture,target,start => AbsentResponse(start,target,visibility));
    }

    private static JsonObject AbsentResponse(ProcessStartInfo start, SafeDeleteGitHubTarget target, string visibility = "public", bool fork = false, int forks = 0)
    {
        var response = Response(start,target,visibility,fork,forks);
        if (IsGraph(start)) response["data"]!["repository"]!["ref"] = null;
        return response;
    }

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (with the remote ref initially absent, an unproved or changed closed-world state keeps every local target)")]
    [Theory]
    [InlineData("public","head")]
    [InlineData("public","base")]
    [InlineData("private","head")]
    [InlineData("private","base")]
    public void InitiallyAbsentClosedWorld_OpenOwnHeadOrBaseRetainsEveryLocalTarget(string visibility, string side)
    {
        using var fixture = new SafeDeleteTests();
        PrepareInitiallyAbsent(fixture,visibility,true);
        fixture.Dependencies.CheckGitHub = target => Check(fixture,target,start =>
        {
            var response = AbsentResponse(start,target,visibility);
            if (IsOwn(start)) SetNodes(response,"own",Pull("R_selected","owner/repo",side == "head" ? "topic" : "other","R_selected","owner/repo",side == "base" ? "topic" : "main"));
            return response;
        });
        Assert.Equal(2,fixture.Run("worktree",fixture.Tree));
        Assert.Contains("open PR " + side,fixture.Output);
        AssertInitiallyAbsentLocalIntact(fixture);
        Assert.Empty(SafeDeleteTests.Git(fixture.Remote,"for-each-ref","refs/heads/topic"));
        Assert.False(Directory.Exists(fixture.ArchiveRoot));
    }

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (with the remote ref initially absent, an unproved or changed closed-world state keeps every local target)")]
    [Theory]
    [InlineData("public",true,0)]
    [InlineData("public",false,1)]
    [InlineData("private",true,0)]
    [InlineData("private",false,1)]
    [InlineData("internal",false,0)]
    public void InitiallyAbsentOutsideClosedWorld_RefusesEveryLocalTarget(string visibility, bool fork, int forks)
    {
        using var fixture = new SafeDeleteTests();
        PrepareInitiallyAbsent(fixture,visibility,true);
        fixture.Dependencies.CheckGitHub = target => Check(fixture,target,start => AbsentResponse(start,target,visibility,fork,forks));
        Assert.Equal(2,fixture.Run("worktree",fixture.Tree));
        AssertInitiallyAbsentLocalIntact(fixture);
        Assert.Empty(SafeDeleteTests.Git(fixture.Remote,"for-each-ref","refs/heads/topic"));
        Assert.False(Directory.Exists(fixture.ArchiveRoot));
    }

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (with the remote ref initially absent, an unproved or changed closed-world state keeps every local target)")]
    [Theory]
    [InlineData("public","before-worktree-delete","ref")]
    [InlineData("private","before-worktree-delete","ref")]
    [InlineData("public","before-local-delete","ref")]
    [InlineData("private","before-local-delete","ref")]
    [InlineData("public","before-worktree-delete","head")]
    [InlineData("private","before-worktree-delete","base")]
    [InlineData("public","before-local-delete","head")]
    [InlineData("private","before-local-delete","base")]
    [InlineData("public","before-worktree-delete","API ref")]
    [InlineData("private","before-local-delete","API failure")]
    public void InitiallyAbsent_FinalLocalStepRechecksRefAndAssociations(string visibility, string point, string change)
    {
        using var fixture = new SafeDeleteTests();
        var worktree = point == "before-worktree-delete";
        PrepareInitiallyAbsent(fixture,visibility,worktree);
        var changed = false;
        fixture.Dependencies.Checkpoint = current =>
        {
            if (current != point) return;
            changed = true;
            if (change == "ref") SafeDeleteTests.Git(fixture.Remote,"update-ref","refs/heads/topic",SafeDeleteTests.Git(fixture.Main,"rev-parse","topic"));
        };
        fixture.Dependencies.CheckGitHub = target => Check(fixture,target,start => FinalAbsentResponse(start,target,visibility,changed,change));
        Assert.Equal(2,fixture.Run(worktree ? ["worktree",fixture.Tree] : ["branch","topic"]));
        Assert.True(fixture.Has("refs/heads/topic"));
        if (worktree) AssertInitiallyAbsentLocalIntact(fixture);
        Assert.Equal(change == "ref",SafeDeleteTests.Git(fixture.Remote,"for-each-ref","refs/heads/topic").Length > 0);
        Assert.Contains("Verified archive",fixture.Output);
        Assert.DoesNotContain("remote deleted",fixture.Output);
    }

    private static JsonObject FinalAbsentResponse(ProcessStartInfo start, SafeDeleteGitHubTarget target, string visibility, bool changed, string change)
    {
        if (changed && change == "API ref") return Response(start,target,visibility);
        var response = AbsentResponse(start,target,visibility);
        if (!changed || !IsOwn(start)) return response;
        if (change == "API failure") response["errors"] = new JsonArray(new JsonObject { ["message"] = "denied" });
        if (change is "head" or "base") SetNodes(response,"own",Pull("R_selected","owner/repo",change == "head" ? "topic" : "other","R_selected","owner/repo",change == "base" ? "topic" : "main"));
        return response;
    }

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (with the remote ref initially absent, an unproved or changed closed-world state keeps every local target)")]
    [Theory]
    [InlineData("public",true,"fork")]
    [InlineData("public",true,"forks")]
    [InlineData("private",true,"fork")]
    [InlineData("private",true,"forks")]
    [InlineData("public",false,"fork")]
    [InlineData("public",false,"forks")]
    [InlineData("private",false,"fork")]
    [InlineData("private",false,"forks")]
    public void InitiallyAbsent_FinalMetadataReadRefusesAuthorityChange(string visibility, bool worktree, string change)
    {
        using var fixture = new SafeDeleteTests();
        PrepareInitiallyAbsent(fixture,visibility,worktree);
        var final = false;
        var metadataReads = 0;
        fixture.Dependencies.Checkpoint = point =>
        {
            if (point == (worktree ? "before-worktree-delete" : "before-local-delete")) final = true;
        };
        fixture.Dependencies.CheckGitHub = target => Check(fixture,target,start =>
        {
            if (final && !IsGraph(start)) metadataReads++;
            var changed = metadataReads == 2;
            return AbsentResponse(start,target,visibility,changed && change == "fork",changed && change == "forks" ? 1 : 0);
        });
        Assert.Equal(2,fixture.Run(worktree ? ["worktree",fixture.Tree] : ["branch","topic"]));
        Assert.Equal(2,metadataReads);
        Assert.True(fixture.Has("refs/heads/topic"));
        if (worktree) AssertInitiallyAbsentLocalIntact(fixture);
        Assert.Empty(SafeDeleteTests.Git(fixture.Remote,"for-each-ref","refs/heads/topic"));
        Assert.Contains("Verified archive",fixture.Output);
    }

    private static void AssertInitiallyAbsentLocalIntact(SafeDeleteTests fixture)
    {
        Assert.Equal(SafeDeleteTests.Git(fixture.Main,"rev-parse","main"),SafeDeleteTests.Git(fixture.Main,"rev-parse","topic"));
        Assert.Equal("staged only",SafeDeleteTests.Git(fixture.Tree,"show",":file.txt"));
        Assert.Equal("working bytes\n",File.ReadAllText(Path.Combine(fixture.Tree,"file.txt")));
        var paths = SafeDeleteTests.Git(fixture.Main,"worktree","list","--porcelain","-z")
            .Split('\0',StringSplitOptions.RemoveEmptyEntries)
            .Where(field => field.StartsWith("worktree ",StringComparison.Ordinal))
            .Select(field => field[9..].Replace('\\','/'));
        Assert.Contains(fixture.Tree.Replace('\\','/'),paths,
            OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
    }

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (with the remote ref initially absent, an unproved or changed closed-world state keeps every local target)")]
    [Theory]
    [InlineData("errors")]
    [InlineData("data null")]
    [InlineData("repository null")]
    [InlineData("repo name")]
    [InlineData("repo id")]
    [InlineData("own base identity")]
    [InlineData("own owner identity")]
    [InlineData("count short")]
    [InlineData("nodes null")]
    public void InitiallyAbsent_MalformedOwnListingCannotProveAbsence(string defect)
    {
        using var fixture = new SafeDeleteTests();
        PrepareInitiallyAbsent(fixture,"public",true);
        fixture.Dependencies.CheckGitHub = target => Check(fixture,target,start =>
        {
            var response = AbsentResponse(start,target);
            if (IsOwn(start)) Damage(response,start,defect);
            return response;
        });
        Assert.Equal(2,fixture.Run("worktree",fixture.Tree));
        AssertInitiallyAbsentLocalIntact(fixture);
        Assert.Empty(SafeDeleteTests.Git(fixture.Remote,"for-each-ref","refs/heads/topic"));
        Assert.False(Directory.Exists(fixture.ArchiveRoot));
    }

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (own-PR pagination proves eligibility only when it is terminal, unique and complete)")]
    [Theory]
    [InlineData("complete",false,false)]
    [InlineData("late base",true,false)]
    [InlineData("repeated cursor",true,false)]
    [InlineData("missing cursor",true,false)]
    [InlineData("changing count",true,false)]
    [InlineData("duplicate node",true,false)]
    [InlineData("page limit",true,false)]
    [InlineData("complete",false,true)]
    [InlineData("late head",true,true)]
    [InlineData("late base",true,true)]
    [InlineData("repeated cursor",true,true)]
    [InlineData("missing cursor",true,true)]
    [InlineData("changing count",true,true)]
    [InlineData("duplicate node",true,true)]
    [InlineData("page limit",true,true)]
    public void OwnPullPagination_RequiresTerminalUniqueCompleteCount(string variant, bool refused, bool initiallyAbsent)
    {
        using var fixture = new SafeDeleteTests();
        var target = new SafeDeleteGitHubTarget("https://github.com/owner/repo.git","topic",initiallyAbsent ? null : Target(fixture).Oid);
        var page = 0;
        var result = Check(fixture,target,start =>
        {
            var response = initiallyAbsent ? AbsentResponse(start,target) : Response(start,target);
            if (!IsOwn(start)) return response;
            page++;
            PaginationResponse(response,variant,page);
            if (page > 1)
            {
                var previous = variant == "repeated cursor" ? "cursor" : "cursor-" + (page - 1);
                Assert.Contains("endCursor=" + previous,start.ArgumentList);
            }
            return response;
        });
        Assert.Equal(refused,result != null);
        var expected = variant == "missing cursor" ? 1 : 2;
        if (variant == "page limit") expected = 100;
        Assert.Equal(expected,page);
    }

    private static void PaginationResponse(JsonObject response, string variant, int page)
    {
        var connection = response["data"]!["repository"]!["pullRequests"]!;
        var pr = Pull("R_other","other/repo","topic","R_selected","owner/repo",variant == "late base" && page == 2 ? "topic" : "main");
        if (variant == "late head" && page == 2) pr = Pull();
        pr["id"] = variant == "duplicate node" ? "PR_same" : "PR_" + page;
        SetNodes(response,"own",pr);
        connection["totalCount"] = variant == "page limit" ? 101 : 2;
        if (variant == "changing count" && page == 2) connection["totalCount"] = 3;
        connection["pageInfo"]!["hasNextPage"] = page == 1 || variant is "repeated cursor" or "page limit";
        connection["pageInfo"]!["endCursor"] = variant == "missing cursor" ? null : "cursor-" + page;
        if (variant == "repeated cursor") connection["pageInfo"]!["endCursor"] = "cursor";
    }

    [Trait("Job", "contract: spec N10; DYD-1029, DYD-1142 (remote cleanup only on proved closed-world state)")]
    [Fact]
    public void ProcessFailuresAndMalformedOrOversizedOutput_Refuse()
    {
        using var fixture = new SafeDeleteTests(); var target = Target(fixture);
        Assert.Contains("unsupported",SafeDeleteGitHub.Check(new("file:///local.git","topic",target.Oid)));
        Assert.Contains("protected",SafeDeleteGitHub.Check(new(target.Url,"feature/integration",target.Oid)));
        Assert.Contains("failed",SafeDeleteGitHub.CheckUsing(new(target.Url,"topic",null),_ => Start(fixture,"not-a-git-command")));
        Assert.Contains("failed",SafeDeleteGitHub.CheckUsing(target,_ => Start(fixture,"not-a-git-command")));
        Assert.Contains("unavailable",SafeDeleteGitHub.CheckUsing(target,_ => new ProcessStartInfo("dydo-nonexistent-gh") { UseShellExecute = false }));
        Assert.Contains("unavailable",Raw(fixture,target,"not json"));
        Assert.Contains("byte budget",Raw(fixture,target,new string('x',8 * 1024 * 1024 + 1)));
    }

    private static JsonObject Damage(JsonObject response, ProcessStartInfo start, string defect)
    {
        if (!IsGraph(start)) return DamageMetadata(response,defect);
        var repo = response["data"]!["repository"]!;
        var reference = repo["ref"]!;
        var conn = IsHeads(start) ? reference["associatedPullRequests"]! : repo["pullRequests"]!;
        switch (defect)
        {
            case "errors": response["errors"] = new JsonArray(new JsonObject { ["message"] = "partial data" }); break;
            case "errors malformed": response["errors"] = new JsonObject(); break;
            case "data null": response["data"] = null; break;
            case "repository null": response["data"]!["repository"] = null; break;
            case "ref null": repo["ref"] = null; break;
            case "ref branch": reference["name"] = "other"; break;
            case "ref prefix": reference["prefix"] = "refs/tags/"; break;
            case "ref oid": reference["target"]!["oid"] = new string('b',40); break;
            case "ref repository": reference["repository"]!["id"] = "R_other"; break;
            case "ref id": reference["id"] = null; break;
            case "repo name": repo["nameWithOwner"] = "other/repo"; break;
            case "repo id": repo["id"] = "R_other"; break;
            case "count missing": conn.AsObject().Remove("totalCount"); break;
            case "count negative": conn["totalCount"] = -1; break;
            case "count fractional": conn["totalCount"] = 1.5; break;
            case "count short": conn["totalCount"] = 1; break;
            case "nodes null": conn["nodes"] = null; break;
            case "cursor missing": conn["pageInfo"]!.AsObject().Remove("endCursor"); break;
            case "cursor null": case "cursor wrong type": case "empty nonterminal":
                conn["totalCount"] = 2; conn["pageInfo"]!["hasNextPage"] = true;
                conn["pageInfo"]!["endCursor"] = defect == "cursor wrong type" ? JsonValue.Create(42) : null;
                break;
            default: DamagePull(response,start,defect); break;
        }
        return response;
    }

    private static JsonObject DamageMetadata(JsonObject response, string defect)
    {
        switch (defect)
        {
            case "private fork": response["visibility"] = "private"; response["private"] = true; response["fork"] = true; break;
            case "private forks": response["visibility"] = "private"; response["private"] = true; response["forks_count"] = 1; break;
            case "internal": response["visibility"] = "internal"; response["private"] = true; break;
            case "private mismatch": response["private"] = true; break;
            case "missing forks": response.Remove("forks_count"); break;
            case "metadata repo": response["full_name"] = "other/repo"; break;
            case "metadata id": response["node_id"] = "R_replaced"; break;
            case "negative forks": response["forks_count"] = -1; break;
            case "fractional forks": response["forks_count"] = 0.5; break;
            case "visibility": response["visibility"] = "unknown"; response["private"] = true; break;
        }
        return response;
    }

    private static void DamagePull(JsonObject response, ProcessStartInfo start, string defect)
    {
        var own = defect.StartsWith("own",StringComparison.Ordinal);
        if (own != IsOwn(start)) return;
        JsonObject? pr = Pull();
        switch (defect)
        {
            case "pr null": pr = null; break;
            case "pr state": pr["state"] = "CLOSED"; break;
            case "pr head null": pr["headRepository"] = null; break;
            case "pr base null": pr["baseRepository"] = null; break;
            case "pr id": pr["id"] = null; break;
            case "pr branch": pr["headRefName"] = "other"; break;
            case "pr head identity": pr["headRepository"]!["id"] = "R_other"; break;
            case "own base identity": pr["baseRepository"]!["id"] = "R_other"; break;
            case "own owner identity": pr["repository"]!["id"] = "R_other"; break;
            default: return;
        }
        SetNodes(response,own ? "own" : "heads",pr);
    }

    private static void Refusal(Func<ProcessStartInfo,SafeDeleteGitHubTarget,JsonObject> response, string reason)
    {
        using var fixture = new SafeDeleteTests(); fixture.AddRemote(); fixture.AddTree();
        File.WriteAllText(Path.Combine(fixture.Tree,"unique.txt"),"unique fork bytes\n");
        fixture.Dependencies.CheckGitHub = target => Check(fixture,target,start => response(start,target));
        Assert.Equal(2,fixture.Run("worktree",fixture.Tree));
        Assert.Contains(reason,fixture.Output);
        Intact(fixture);
        Assert.Equal("unique fork bytes\n",File.ReadAllText(Path.Combine(fixture.Tree,"unique.txt")));
        Assert.False(Directory.Exists(fixture.ArchiveRoot));
    }

    private static void Intact(SafeDeleteTests fixture)
    {
        var tip = SafeDeleteTests.Git(fixture.Main,"rev-parse","main");
        Assert.Equal(tip,SafeDeleteTests.Git(fixture.Main,"rev-parse","topic"));
        Assert.Equal(tip,SafeDeleteTests.Git(fixture.Remote,"rev-parse","topic"));
        Assert.Equal("base\n",File.ReadAllText(Path.Combine(fixture.Tree,"file.txt")));
        Assert.True(Directory.Exists(fixture.Tree));
    }

    private static SafeDeleteGitHubTarget Target(SafeDeleteTests fixture) => new("https://github.com/owner/repo.git","topic",SafeDeleteTests.Git(fixture.Main,"rev-parse","topic"));

    private static string? Check(SafeDeleteTests fixture, SafeDeleteGitHubTarget target, Func<ProcessStartInfo,JsonObject> response)
    {
        // Only transport is local-bare. The external API process and its validated
        // protocol stay production code; the same pinned identity/receipt crosses calls.
        var apiTarget = new SafeDeleteGitHubTarget("https://github.com/owner/repo.git",target.Branch,target.Oid)
        {
            Metadata = target.Metadata, DeletedByInvocation = target.DeletedByInvocation
        };
        var result = SafeDeleteGitHub.CheckUsing(apiTarget,start => Output(fixture,response(start).ToJsonString()));
        target.Metadata = apiTarget.Metadata;
        return result;
    }

    private static string? Raw(SafeDeleteTests fixture, SafeDeleteGitHubTarget target, string response)
        => SafeDeleteGitHub.CheckUsing(target,_ => Output(fixture,response));

    private static ProcessStartInfo Output(SafeDeleteTests fixture, string response)
    {
        var config = Path.Combine(fixture.Root,"eligibility.config");
        File.WriteAllText(config,"[api]\nresponse = \"" + response.Replace("\\","\\\\").Replace("\"","\\\"").Replace("\n","\\n").Replace("\r","\\n") + "\"\n");
        return Start(fixture,"config","--file",config,"--get","api.response");
    }

    private static ProcessStartInfo Start(SafeDeleteTests fixture, params string[] arguments)
    {
        var start = new ProcessStartInfo("git") { WorkingDirectory = fixture.Main, UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true };
        foreach (var argument in arguments) start.ArgumentList.Add(argument);
        return start;
    }

    private static bool IsGraph(ProcessStartInfo start) => start.ArgumentList.Contains("graphql");
    private static bool IsHeads(ProcessStartInfo start) => IsGraph(start) && start.ArgumentList.Any(arg => arg.StartsWith("query=",StringComparison.Ordinal) && arg.Contains("associatedPullRequests",StringComparison.Ordinal));
    private static bool IsOwn(ProcessStartInfo start) => IsGraph(start) && !IsHeads(start);

    private static JsonObject Response(ProcessStartInfo start, SafeDeleteGitHubTarget target, string visibility = "public", bool fork = false, int forks = 0, bool? apiRefPresent = null)
    {
        Assert.Equal("gh",start.FileName);
        Assert.Contains("github.com",start.ArgumentList);
        if (!IsGraph(start))
        {
            Assert.Contains("repos/owner/repo",start.ArgumentList);
            return new JsonObject { ["node_id"] = "R_selected", ["full_name"] = "owner/repo", ["visibility"] = visibility, ["private"] = visibility != "public", ["fork"] = fork, ["forks_count"] = forks };
        }
        Assert.Contains("qualifiedName=refs/heads/topic",start.ArgumentList);
        var repo = new JsonObject { ["id"] = "R_selected", ["nameWithOwner"] = "owner/repo" };
        var reference = new JsonObject { ["id"] = "REF_topic", ["name"] = "topic", ["prefix"] = "refs/heads/", ["repository"] = repo.DeepClone(), ["target"] = new JsonObject { ["oid"] = target.Oid } };
        if (IsHeads(start)) reference["associatedPullRequests"] = EmptyConnection();
        else repo["pullRequests"] = EmptyConnection();
        repo["ref"] = (apiRefPresent ?? !target.DeletedByInvocation) ? reference : null;
        return new JsonObject { ["data"] = new JsonObject { ["repository"] = repo } };
    }

    private static JsonObject EmptyConnection() => new() { ["totalCount"] = 0, ["nodes"] = new JsonArray(), ["pageInfo"] = new JsonObject { ["hasNextPage"] = false, ["endCursor"] = null } };

    private static JsonObject Pull(string headId = "R_selected", string headRepo = "owner/repo", string headName = "topic", string baseId = "R_selected", string baseRepo = "owner/repo", string baseName = "main")
        => new() { ["id"] = "PR_one", ["state"] = "OPEN", ["headRefName"] = headName, ["headRefOid"] = new string('a',40), ["headRepository"] = new JsonObject { ["id"] = headId, ["nameWithOwner"] = headRepo }, ["baseRefName"] = baseName, ["baseRepository"] = new JsonObject { ["id"] = baseId, ["nameWithOwner"] = baseRepo }, ["repository"] = new JsonObject { ["id"] = baseId, ["nameWithOwner"] = baseRepo } };

    private static void SetNodes(JsonObject response, string field, JsonObject? pr)
    {
        var repo = response["data"]!["repository"]!;
        var connection = field == "own" ? repo["pullRequests"]! : repo["ref"]!["associatedPullRequests"]!;
        connection["totalCount"] = 1;
        connection["nodes"] = new JsonArray(pr);
    }
}
