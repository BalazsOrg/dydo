namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Charts;

/// <summary>
/// <c>claims/statechart/&lt;machine&gt;/transitions.json</c> (DYD-1082 contract, "Statechart model"; K1, T15):
/// a machine's states, variables, transitions with their claim ids and carrying skills, and gaps.
/// </summary>
public class TransitionsJsonTests
{
    [Fact]
    [Trait("Job", "contract: K1 transitions.json lists every state, transition and gap with claim and spec ids")]
    public void LeaseTransitions_ListTheWholeMachine() => Assert.Equal(StatechartFilesTests.Text("""
        {
          "$comment": "built from the model; edit there: tools/SkillBuild",
          "id": "Lease",
          "title": "Lease",
          "initial": "Free",
          "finals": [
            "Done"
          ],
          "states": [
            {
              "id": "Free",
              "title": "Free",
              "spec": [
                "K1"
              ]
            },
            {
              "id": "Held",
              "title": "Held",
              "spec": [
                "K9"
              ]
            },
            {
              "id": "Review",
              "title": "In review",
              "spec": [
                "R1"
              ]
            },
            {
              "id": "Done",
              "title": "Done",
              "spec": [
                "R2",
                "N12 DR 049 §2–4"
              ]
            }
          ],
          "variables": [
            {
              "name": "fixes",
              "initial": "0",
              "bound": "fixes <= 2"
            }
          ],
          "transitions": [
            {
              "claim": "claim:LeaseMoves.Fail",
              "from": "Review",
              "to": "Held",
              "arrow": "the review fails",
              "spec": [
                "R3"
              ],
              "guard": "fixes < 2",
              "effect": "fixes' = fixes + 1",
              "fair": false,
              "skills": [
                "holder",
                "warden"
              ]
            },
            {
              "claim": "claim:LeaseMoves.Land",
              "from": "Review",
              "to": "Done",
              "arrow": "the merge lands",
              "spec": [
                "R2",
                "CA19"
              ],
              "guard": null,
              "effect": null,
              "fair": true,
              "skills": []
            },
            {
              "claim": "claim:LeaseMoves.Return",
              "from": "Held",
              "to": "Review",
              "arrow": "the holder returns it",
              "spec": [
                "R1",
                "K28"
              ],
              "guard": null,
              "effect": null,
              "fair": false,
              "skills": [
                "holder",
                "warden"
              ]
            },
            {
              "claim": "claim:LeaseMoves.Take",
              "from": "Free",
              "to": "Held",
              "arrow": "the holder takes the lease",
              "spec": [
                "K9"
              ],
              "guard": null,
              "effect": null,
              "fair": false,
              "skills": [
                "holder"
              ]
            }
          ],
          "gaps": [
            {
              "action": "Gap1",
              "from": "Held",
              "to": "Free",
              "note": "the spec names no release of a held lease"
            },
            {
              "action": "Gap2",
              "from": "Review",
              "to": null,
              "note": "the spec names no move after a second failed fix"
            }
          ]
        }
        """), SkillBuilder.Build(ChartModel.With()).Files["claims/statechart/lease/transitions.json"]);

    [Fact]
    [Trait("Job", "contract: K6 transitions.json carries the built header as its first member, so it is a built file")]
    public void TransitionsJson_CarriesTheBuiltHeader()
    {
        using var root = new TempRoot();
        root.Run(ChartModel.With());

        Assert.True(BuiltHeader.Carries(root["claims/statechart/lease/transitions.json"]));
        Assert.Equal("  " + BuiltHeader.Json, File.ReadLines(root["claims/statechart/lease/transitions.json"]).ElementAt(1));
    }
}
