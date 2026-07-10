# Module Verification Report

## PURPOSE

<!-- GUIDANCE: <Describe the purpose of this report.> -->
<!-- GUIDANCE: [Example: This document summarizes the execution results of <Project Name> Module Test or Integration Plan.] -->

## SCOPE

<!-- GUIDANCE: <Describe the list of modules tested as part of this report. List the modules being tested in the case of Module-Integration or the single module being tested for module verification.> -->
<!-- GUIDANCE: This record applies to Philips, CT/AMI. -->

## Technical Administrative Information

<!-- GUIDANCE: Date -->
<!-- GUIDANCE: Agenda -->
<!-- GUIDANCE: Attendance -->
<!-- GUIDANCE: [All invitees to this review must be documented within this table. Review can be performed offline. Add lines as necessary.] -->

| Capacity/Function of the Reviewer(s)) | Attendance |
| --- | --- |

<!-- GUIDANCE: Review Outcome -->
<!-- GUIDANCE: [List open issues or comments if necessary. If No comments, write “<Module verification/Module-Integration> Report was reviewed and approved. There are no open items associated with this report.”] -->

## CONTENT

<!-- GUIDANCE: [This section shall document the execution and results of the tests completed. If the execution of the tests is done external to this document, this section shall be removed and replaced by appropriate reference either to an external document or an appendix. Add as many major test sections or test sub sections as needed. Major Test and Sub Test section numbering shall be aligned with the Module Test or module Integration Procedure.] -->
<!-- GUIDANCE: repeatable example section: <Major Test Section 1> -->
<!-- GUIDANCE: [A Major Test Section would include a primary part of a module or module group that is large enough to need separate smaller (sub-tests) generated in order to provide sufficient coverage of the module or module group. -->
<!-- GUIDANCE: Major Test Section 1 may relate to specific SW Module or HW part, where dedicated Sub Test sections may specify groups or cycles of tests planned for completion of coverage. A Major Test Section 2 may be established for another module or HW part.] -->

#### Test Prerequisites

<!-- GUIDANCE: [This section lists information about the overall setup of the system prior to running the tests. For example: The following test cases assume the tester has logged into the Server Workstation as “patient”.] -->
<!-- GUIDANCE: [Fill in the n next to the “Test Run” header to match the Timeline table above.] -->
<!-- GUIDANCE: For tests involving software, record the release IDs of the programs/scripts: -->
<!-- GUIDANCE: [Add or remove rows in the table below as necessary.] -->
<!-- GUIDANCE: SW Executables: -->

| Test Run <n> |
| --- |

<!-- GUIDANCE: Test Equipment/setup: -->
<!-- GUIDANCE: [This section should list the various pieces of test equipment used for this Test Run. All equipment should be in calibrated with the date of the last calibration supplied below.] -->
<!-- GUIDANCE: [Fill in the n next to the “Test Run” header to match the Timeline table above.] -->
<!-- GUIDANCE: [If section is not applicable, e.g. for SW testing, section can be removed] -->
<!-- GUIDANCE: [Add or remove rows in the table below as necessary.] -->

| Test Run <n> |
| --- |

<!-- GUIDANCE: <Sub Test Section 1> -->
<!-- GUIDANCE: [The Sub Test Sections are to allow a breakdown of the module or module group into smaller areas for testing purposes. Example: For a module, a Major Test Section could be GUI presentation and then a set of Sub Test Sections to test separate parts of the GUI, such as each screen display. When using template for SW WIP Sanity Testing, list of ARs covered by tested WIP needs to be listed within the report] -->
<!-- GUIDANCE: Objective: <Short statement of the objective of this test section> -->
<!-- GUIDANCE: AR: <number of Defect or EI that is covered by this test if applicable. Otherwise state N/A> -->
<!-- GUIDANCE: Setup: <Add specific setup details applicable of Sub Test. If no additional details are required, write N/A > -->
<!-- GUIDANCE: Detailed Description: <if applicable, add detailed information relevant to this particular test section 1. Otherwise state N/A> -->
<!-- GUIDANCE: [Add or remove rows in the table below as necessary.] -->

| Test ID/ Name | Operator Actions | Expected Results [Note: State acceptance criteria objectively. Only include discrete quantitative or qualitative acceptance criteria that do not require judgment to determine pass or fail.] | Actual Results and evidence | Tester ID or Name | Execution Date | Status (Pass / Fail / Not Run) | Comments | Change Request Number or AR number (if applicable) |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |

<!-- GUIDANCE: <Note: Status outcome of Test Results can be either “Pass” or “Not Run” or “Fail”. Justification for “Not Run” and clarification for “Fail” shall be provided in Comments column.> -->

### Timeline

<!-- GUIDANCE: [This section is optional, If table below is not used specify “N/A” ] -->
<!-- GUIDANCE: [This section lists a numeric value (n) for each Test Run along with the start and end date for that run. The Test Run number (n) is used in subsequent tables to reference the sequential test runs in this summary.] -->
<!-- GUIDANCE: [Add or remove rows in the table below as necessary.] -->

| Test Run | Actual Test Run Start Date | Actual Test Run Completion Date | Description [Optional] |
| --- | --- | --- | --- |

### Module Verification Configuration

#### System/Subsystem Configuration

<!-- GUIDANCE: [This report generally assumes a default, production configured system is used for the module verification. If this is not available, then provide enough reference information below to describe the configuration of the system (or part of a system) that will be used for the testing.] -->
<!-- GUIDANCE: [Fill in the number next to the “Test Run” header to match the Timeline table above – if applicable.] -->
<!-- GUIDANCE: [Add or remove rows in the table below as necessary.] -->

| Test Run <n> |
| --- |

### Module-Integration Test Coverage

<!-- GUIDANCE: [This section applies for Module-Integration Report only. Specify “N/A” if purpose doesn’t include Integration activities] -->
<!-- GUIDANCE: [The use of this table is optional and up to the discretion of the Development Engineer / Test Engineer.] -->
<!-- GUIDANCE: [If generating a Module-Integration Report, then the pertinent information may be listed in the table below. This table is provided to indicate the amount and type of test coverage that was provided for Module Groups as identified in the Module-Integration Plan in the “Modules to be Tested” section.] -->
<!-- GUIDANCE: [Fill in the n next to the “Test Run” header to match the Timeline table above.] -->
<!-- GUIDANCE: [Add or remove rows in the table below as necessary.] -->

| Test Run <n> |
| --- |

### Anomaly Record (AR) and Change Requests Identified

<!-- GUIDANCE: [Provide a summary listing of the change requests that were generated as a result of this Test Run for any test case that “Failed”.] -->
<!-- GUIDANCE: [Fill in the n next to the “Test Run” header to match the Timeline table above – if applicable.] -->
<!-- GUIDANCE: [Add or remove rows in the table below as necessary.] -->

| Test Run <n> |
| --- |

### Test Variances

<!-- GUIDANCE: [Indicate all deviations from the test script and reasons for their occurrence. When a deviation from the script prevents steps from being executed to the extent specified in the test script, the test shall be redesigned, reapproved and executed] -->
<!-- GUIDANCE: [Fill in the n next to the “Test Run” header to match the Timeline table above.] -->
<!-- GUIDANCE: [Add or remove rows in the table below as necessary.] -->

| Test Run <n> |
| --- |

### Traceability

<!-- GUIDANCE: [Traceability from Module verification to requirements shall be demonstrated. Traceability can be provided as an appendix that can be attached to this document or by an accompanying TRR. If Traceability is provided by a separated document or an appendix, the table below can be removed and a reference to the appendix or TRR shall be added.] -->
<!-- GUIDANCE: [The minimum information to be captured for report level Traceability Matrix is listed below. For SW Modules, traceability of procedure/report to SW Module name is sufficient] -->

| Requirement ID or Spec ID [for SW Modules traceability, specify design document DHF] | Module/Unit/ Component | Test ID/Test Name | Test Status |
| --- | --- | --- | --- |

### Conclusion

<!-- GUIDANCE: [Summarize the results of the module verification or module integration in this paragraph. State whether the testing was considered successful and the reasons for this assertion. It may be prudent to give a recommendation as to whether the next steps in the testing process (i.e.: A subsequent test run, System Integration, Verification, etc.) are ready for commencement, based on the results of this and any other applicable Module and/or Module-Integration Test Reports.] -->
<!-- GUIDANCE: [Add or remove rows in the table below as necessary, below is an example only.] -->
<!-- GUIDANCE: <Example: All tests, as described in DHFXXXX, have passed successfully.> -->
<!-- GUIDANCE: <Example: Test XXX have passed successfully, Test Number XXX has failed and needs to be re-run.> -->

## TERMS AND ABBREVIATIONS

<!-- GUIDANCE: [Include applicable definitions as necessary. Add or remove rows in the table below as necessary] -->

| Term / Abbreviation | Description |
| --- | --- |

## APPENDICES

<!-- GUIDANCE: [The appendix should be considered only additional supporting information or guidance. If no appendices are included, populate the table below with “N/A” or “Not Applicable”.] -->
<!-- GUIDANCE: [Add or remove rows in the table below as necessary] -->

| Appendix | Title |
| --- | --- |

## REFERENCES

<!-- GUIDANCE: [Include applicable references as necessary. Add or remove rows in the table below as necessary.] -->

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

<!-- GUIDANCE: All testers that record tests manually (not in ALM) shall sign the DCO and be identified in the below table. Their signatures confirm that the tests were recorded by them on the listed date -->
<!-- GUIDANCE: Signatures and dates are captured in PLM tool as part of the document change order. -->

| Signature Reason | Function | Name |
| --- | --- | --- |

<!-- GUIDANCE: repeatable example section: APPENDIX <X> – < NAME OF THE APPENDIX > -->
