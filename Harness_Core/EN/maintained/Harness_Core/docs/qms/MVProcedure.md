# Module Verification Procedure

## PURPOSE

<!-- GUIDANCE: This document defines the sequential steps and expected test results for <insert Project Name here > < select: Module Verification or Module-Integration > -->

## SCOPE

<!-- GUIDANCE: <Describe the list of modules planned for testing as part of this procedure.> -->
<!-- GUIDANCE: This record applies to Philips, CT/AMI. -->

## Technical Administrative Information

<!-- GUIDANCE: Date -->
<!-- GUIDANCE: Agenda -->
<!-- GUIDANCE: Attendance -->
<!-- GUIDANCE: [All invitees to this review must be documented within this table. Review can be performed offline. Add lines as necessary.] -->

| Capacity/Function of the Reviewer(s)) | Attendance |
| --- | --- |

<!-- GUIDANCE: Review Outcome -->
<!-- GUIDANCE: [List open issues or comments if necessary. If No comments during the procedure review, write “<Module verification/Module-Integration> Procedure was reviewed and approved. There are no open items associated with this procedure.”] -->

## CONTENT

<!-- GUIDANCE: [This section elaborates the content of reviewed and approved verification or integration activities that will be performed as part of module verification or/and module-integration. If the tests are managed and approved via a management tool then chapter 3.1 shall be removed and replaced by appropriate reference to an Appendix or an external document. The appendix of this document includes recommended guidance for items to be considered when creating software module verification or module-integration test cases.] -->
<!-- GUIDANCE: [General Template Instruction: -->
<!-- GUIDANCE: Add as many Major Test Sections and sub test sections as needed. -->
<!-- GUIDANCE: The author is requested to fill in: Test Number, Operator Action and Expected Results data. -->
<!-- GUIDANCE: There shall be specific criteria for exiting the unit testing activity (i.e. a coverage threshold, a non-peer review of results, etc.). -->
<!-- GUIDANCE: There shall be either a checklist or a procedure identifying the expected results. For SW module verification or integration, it's not sufficient to just execute the code as there are defects that don't result in a crash or hang.] -->
<!-- GUIDANCE: repeatable example section: <Major Test Section 1> -->
<!-- GUIDANCE: [A Major Test Section is intended to include a primary part of a module or module group (e.g. SW module or HW part) that is large enough to need separate smaller tests (sub-tests or cycles of tests) in order to cover the module or module group requirements. Example: For a module, a Major Test Section could be GUI presentation and then a set of Sub Test Sections to test separate parts of the GUI, such as each screen display. Add Major Test sections as necessary.] -->

#### Test Prerequisites

<!-- GUIDANCE: [This section lists information about the overall setup of the system prior to running the tests. For example: The following test cases assume the tester has logged into the Server Workstation as “patient”. -->
<!-- GUIDANCE: Test Setup: <Describe any setup required prior to running this test section. Ensure that the sample size of the module(s) under test is sufficient for generating statistically significant results, or provide a justification otherwise.> -->
<!-- GUIDANCE: [For tests involving software, record the release IDs of the programs/scripts in the following table. Otherwise, state N/A] -->
<!-- GUIDANCE: [Add or remove rows in the table below as necessary.] -->
<!-- GUIDANCE: SW Executables: -->

| Index | Executable | Release ID | Revision |
| --- | --- | --- | --- |

<!-- GUIDANCE: repeatable example section: <Sub Test Section 1> -->
<!-- GUIDANCE: [The Sub Test Sections enable a breakdown of the module or module group into smaller areas for testing purposes. Add Sub Test sections as necessary. -->
<!-- GUIDANCE: When using template for SW WIP Sanity Testing, list of ARs covered by tested WIP will be mentioned in the report.] -->
<!-- GUIDANCE: Objective: <Short statement of the objective of this test section> -->
<!-- GUIDANCE: AR: <number of Defect or EI that is covered by this test if applicable. Otherwise state N/A> -->
<!-- GUIDANCE: Setup: <Add specific setup details applicable of Sub Test. If no additional details are required, write N/A > -->
<!-- GUIDANCE: Detailed Description: <if applicable, add detailed information relevant to this particular test section 1. Otherwise state N/A> -->
<!-- GUIDANCE: [Add or remove rows in the table below as necessary.] -->

| Test ID/Name | Operator Actions | Expected Results [Note: State acceptance criteria objectively. Only include discrete quantitative or qualitative acceptance criteria that do not require judgment to determine pass or fail.] |
| --- | --- | --- |

### Traceability

<!-- GUIDANCE: [Provide initial traceability for Module Verification or Module Integration procedure to the applicable requirements. For SW modules testing, traceability from procedure to SW Module Name is sufficient.] -->

| Requirement ID or Spec ID [for SW Modules traceability, specify applicable requirements document DHF] | Module/Unit/ Component | Test ID/Test Name [for SW Modules traceability, specify Module Test procedure section numbers] |
| --- | --- | --- |

## TERMS AND ABBREVIATIONS

| Term / Abbreviation | Description |
| --- | --- |

## APPENDICES

| Appendix | Title |
| --- | --- |

## REFERENCES

### External References

| Document ID | Document Title |
| --- | --- |

### Internal References

| Document ID | Document Title |
| --- | --- |

## RECORD CHANGE SUMMARY

| Revision | Document Change No. | Document Editor | Description of Change |
| --- | --- | --- | --- |

## RECORD APPROVALS

<!-- GUIDANCE: Signatures and dates are captured in PLM tool as part of the document change order. -->

| Signature Reason | Function | Name |
| --- | --- | --- |

<!-- GUIDANCE: repeatable example section: APPENDIX <X> – < NAME OF THE APPENDIX > -->
<!-- GUIDANCE: Delete The Following Pages Before Using This Template. -->

## APPENDIX A – SW MODULE TESTING RECOMMENDED GUIDELINES

<!-- GUIDANCE: Guidance on items to consider when creating software module tests: -->
<!-- GUIDANCE: Unit testing can be performed in two levels: white box (developer’s point of view) and black box (requirements point of view). -->
<!-- GUIDANCE: Unit testing is performed at the module and class level. More than one module can be unit tested together if they are related and if the modules have all been coded at the same time. -->
<!-- GUIDANCE: All new code shall be unit tested as well as any high risk modified or reused code. -->
<!-- GUIDANCE: All tests can be run via a debugger, however, a unit testing tool can be useful for identifying paths and coverage once the paths are executed. -->
<!-- GUIDANCE: Contrary to popular belief you do not need to write stubs to unit test the code if you use a debugger to populate the values needed to test the paths. -->
<!-- GUIDANCE: Explicitly call out Unit testing on the developers' schedule. -->
<!-- GUIDANCE: Unit testing can be measured in terms of branch and line coverage. -->
<!-- GUIDANCE: There shall be an executive summary in table form to show test case and pass/fail/not tested yet status. -->
<!-- GUIDANCE: The Software Development Plan should describe specifically what is to be tested for every unit. -->
<!-- GUIDANCE: Mapping of tested software units to requirements shall be available as part of the module or module-integration testing. -->
<!-- GUIDANCE: The unit testing should not only be standardized in scope but the results should also be reviewed by a Lead Software Engineer. -->
<!-- GUIDANCE: If a tool measures code coverage, take note of it in terms of lines of code or branches after testing is complete. -->
