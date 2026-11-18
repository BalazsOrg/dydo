"""Public unittest IDs shared with the exact Reqnroll scenario bindings."""
import unittest
import test_testing_facade_support as support
import test_testing_facade_helpers_1
import test_testing_facade_helpers_2
import test_testing_facade_cases_01
import test_testing_facade_cases_02
import test_testing_facade_cases_03
import test_testing_facade_cases_04
import test_testing_facade_cases_05
import test_testing_facade_cases_06
import test_testing_facade_cases_07
import test_testing_facade_cases_08


class TestingFacadeTests(
        support.FacadeFixture,
        test_testing_facade_helpers_1.FacadeFixture,
        test_testing_facade_helpers_2.FacadeFixture,
        test_testing_facade_cases_01.Cases.FacadeCases01,
        test_testing_facade_cases_02.Cases.FacadeCases02,
        test_testing_facade_cases_03.Cases.FacadeCases03,
        test_testing_facade_cases_04.Cases.FacadeCases04,
        test_testing_facade_cases_05.Cases.FacadeCases05,
        test_testing_facade_cases_06.Cases.FacadeCases06,
        test_testing_facade_cases_07.Cases.FacadeCases07,
        test_testing_facade_cases_08.Cases.FacadeCases08,
        unittest.TestCase):
    """All original cases, once for the repository facade."""


class PortableTestingFacadeTests(TestingFacadeTests):
    runner = support.PORTABLE


if __name__ == '__main__':
    unittest.main()
