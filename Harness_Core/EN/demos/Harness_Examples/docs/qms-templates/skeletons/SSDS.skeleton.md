# Sub-System Design Specification

## PURPOSE

<!-- GUIDANCE: The purpose of this document is to provide the <System or Sub-System Design Specification> for the product <Product Name>. -->

## SCOPE

<!-- GUIDANCE: This record applies to the Philips CT/AMI. -->
<!-- GUIDANCE: [Provide description of the scope of the design document to detail the boundaries of the design as described by this document] -->

## ARCHITECTURE DESIGN OBJECTIVES

<!-- GUIDANCE: [Author: Lead Designer or Design Authority] -->
<!-- GUIDANCE: [For the sake of consistency throughout the system, it is best for the Lead Designer to author this section. However, for the sake of time constraint and distribution of work, it may be more practical for the Design Authority to complete some or all of the subsections after the Overview subsection. In this case, the Lead Designer should be one of the reviewers.] -->

### Overview

<!-- GUIDANCE: [A short history, background, and use of the system or Sub-System will be presented here. Include a description of how the system or Sub-System fits in the product and, as applicable, a reference to the parent PRS or SDS.] -->

### Assumptions and Constraints

<!-- GUIDANCE: [Any assumptions constraints used in creation of this document are to be listed here. This can include any impact across product lines, existing components that are imposed to be “re-used” and not re-developed, etc.] -->

## SYSTEM ARCHITECTURE

### Design Overview

<!-- GUIDANCE: [This section describes the top-level design of the system including the theory of operation and the system decomposition. Each of the sub-systems is presented and the relationships between them. The interactions and behaviors are handled later. Add other subheadings as needed.] -->
<!-- GUIDANCE: The design overview describes the big picture to help with understanding the details that will follow. A diagram\s is\are used here to help explain the overall system design and relation between the sub-systems. If the project team determines that Sub-System Design Specification documents are warranted or not warranted to properly document the system architecture, that decision is to be stated in this section with appropriate rationale.] -->

### Primary Design Considerations

<!-- GUIDANCE: [Explain any issues of a general nature that need to be understood related to the design. This can include such issues as those related to manufacturing design considerations, service design considerations, cost, reliability, usability etc.] -->
<!-- GUIDANCE: [If applicable, in the case of new technologies to be implemented for the first time in this System or Sub-System design: reference the corresponding Proof of Concept Report(s) for verification that the technology to be employed is within the organization’s competency. This may be discussed under a subsequent “issue” heading as appropriate.] -->
<!-- GUIDANCE: [Add additional issues below, as needed.] -->
<!-- GUIDANCE: repeatable example section: <Issue One> -->
<!-- GUIDANCE: <Add details as needed> -->
<!-- GUIDANCE: repeatable example section: <Issue Two> -->
<!-- GUIDANCE: <Add details as needed> -->

### Design Features

<!-- GUIDANCE: [Description:] -->
<!-- GUIDANCE: [Describe the dynamic model, workflows, or specifically how the Sub-Systems or components work together to achieve the required features. Natural language, graphical models, and use cases can be used, as appropriate. Design features normally focus on Clinical use, but a new product may include other new design feature e.g. License Keys, Remote Service etc…] -->
<!-- GUIDANCE: [Separate subsections can be used for each of the use cases or intended uses. As more features are added to the system or Sub-System, new subsections are added to a newer revision of this document. New features may or may not necessitate a change to the systems or Sub-System’s components that it uses.] -->
<!-- GUIDANCE: [Examples:] -->
<!-- GUIDANCE: [Axial Scans; Cardiac Arrhythmia Handling; Brain Perfusion; Remote Service, New Security Tools; etc.] -->

### Hardware Design

<!-- GUIDANCE: [System Level: Optional. Hardware design is part of the System Architecture, and normally applies to Sub-Systems details. If applicable in the system level, a brief of specific major HW design approaches or concepts can be included, with reference to subsequent more detailed Child documents or with pointers to the implementing Sub-Systems.] -->
<!-- GUIDANCE: repeatable example section: <Hardware Sub-System One> -->
<!-- GUIDANCE: [Include a System/Sub-system overview as applicable. If an SSDS or EDS is written for the sub-system, include the overview, provide a reference to the SSDS or EDS or to other Child Detailed Design Documents, and do not include the detail sections below, as the detail will be in the SSDS or EDS, or in other DHF child documents.] -->
<!-- GUIDANCE: repeatable example section: <Detail or Module One> -->
<!-- GUIDANCE: [Describe details of the sub-system. Describe how the design will address the features, functionality, and performance requirements for the hardware sub-system.] -->
<!-- GUIDANCE: [As appropriate, perform Make, Buy, or Reuse analyses for critical elements of the design. For example, if there is a choice to develop a cable assembly internally or to have it outsourced, describe the rationale for the decision.] -->
<!-- GUIDANCE: [or:] -->
<!-- GUIDANCE: [If an EDS is written for the module, include an overview, provide a reference to the EDS, and do not include the detail, as it will be in the EDS.] -->
<!-- GUIDANCE: repeatable example section: <Detail or Module Two> -->
<!-- GUIDANCE: [Add additional details or modules, as needed.] -->
<!-- GUIDANCE: repeatable example section: <Hardware Sub-System Two> -->
<!-- GUIDANCE: repeatable example section: [Add additional sub-systems, as needed.] -->

### Software Design

<!-- GUIDANCE: [System: An overall description of the software architecture should be provided including a graphical representation of the overall software design, the related sub-systems, and the interfaces between them. The software architecture may be detailed further in the subsequent sections. -->
<!-- GUIDANCE: Sub-System: An overall description of the Sub-System software architecture should be provided including a graphical representation of the software design, the related sub-components, and the interfaces between them. The Sub-Systems software architecture may be detailed further in the subsequent sections.] -->
<!-- GUIDANCE: repeatable example section: <Software Sub-System One> -->
<!-- GUIDANCE: [System Element (SDS only): Show a top level block diagram with the system software architecture that contains the sub-system elements under discussion for this section, the purpose is to identify the implementation of the sub-system element within the overall architecture as well as intra-sub-system interface information such as data flow methodology, interaction technology (such as RPC/P2P Protocol TCP/IP Shared Memory etc…)] -->
<!-- GUIDANCE: [Sub-system element (SSDS only): Include a sub-system overview including a graphical representation of the sub-system design, the related modules, and the interfaces between them. If an SSDS or SwDS is written for the sub-system, include the overview, provide a reference to the SSDS or SwDS, and do not include the detail sections below, as the detail will be in the SSDS or SwDS. Include a definition and/or description of the software sub-system inputs and outputs.] -->
<!-- GUIDANCE: repeatable example section: <Detail or Module One> -->
<!-- GUIDANCE: [Describe details of the sub-system. Describe how the design will address the features, functionality, and performance requirements for the software sub-system. Include ranges, limits defaults and the use of specific values.] -->
<!-- GUIDANCE: [As appropriate, perform Make, Buy, or Reuse analyses for critical elements of the design. For example, if there is a choice to develop a software application internally or to have it outsourced, describe the rationale for the decision.] -->
<!-- GUIDANCE: [or:] -->
<!-- GUIDANCE: [If an SwDS is written for the module, include an overview, provide a reference to the SwDS, and do not include the detail, as it will be in the SwDS.] -->
<!-- GUIDANCE: repeatable example section: <Detail or Module Two> -->
<!-- GUIDANCE: [Add additional details or modules, as needed.] -->
<!-- GUIDANCE: repeatable example section: <Software Sub-System Interface Detail> -->
<!-- GUIDANCE: [Interfaces between this sub-system and other software sub-systems and/or hardware sub-systems that it is associated with should be defined and described.] -->
<!-- GUIDANCE: [or:] -->
<!-- GUIDANCE: [If an SwDS is written for the interface, include an overview, provide a reference to the SwDS, and do not include the detail, as it will be in the SwDS.] -->
<!-- GUIDANCE: repeatable example section: <Software Sub-System Two> -->
<!-- GUIDANCE: [Add sub-systems, as needed.] -->

#### System and Sub-System Software Risk Classification

<!-- GUIDANCE: [SW Classification is shown at the System level with an abstract classification diagram of the Sub-systems to show the classification hierarchy from System down to Sub-system] -->
<!-- GUIDANCE: [Software shall be classified per the Risk Management SOP (2003000438). SW Classification shall be discussed at the System Level (SDS) and/or at the Sub-System Level (SSDS and SwDS) depending on the complexity and the decomposition approach chosen. Software Classification shall be shown in the product Risk Management Matrix in the line item associated with the Software Module. The classification of the software module will be used as part of the design process.] -->
<!-- GUIDANCE: Software Module Classification Table: -->
<!-- GUIDANCE: * This Software Module table is used to describe the system design. This table does not perform the software classification; it only uses the information to further describe design decisions. -->

| Software Module | Software Module Description | Safety Classification (B or C only) |
| --- | --- | --- |

#### Sub-System Software Risk Classification Segregation Rationale

<!-- GUIDANCE: [SW segregation rationale provided here for SW Items that are A and B] -->
<!-- GUIDANCE: [Example provided from this line until the end of this section] -->
<!-- GUIDANCE: <The software ARCHITECTURE should promote segregation of software items that are required for safe operation and should describe the methods used to ensure effective segregation of those SOFTWARE ITEMS. Segregation is not restricted to physical (processor or memory partition) -->
<!-- GUIDANCE: separation but includes any mechanism that prevents one SOFTWARE ITEM from negatively -->
<!-- GUIDANCE: affecting another. The adequacy of a segregation is determined based on the RISKS involved -->
<!-- GUIDANCE: and the rationale which is required to be documented. -->
<!-- GUIDANCE: The Figure below illustrates the possible partitioning for SOFTWARE ITEMS within a SOFTWARE SYSTEM -->
<!-- GUIDANCE: and how the software safety classes would be applied to the group of SOFTWARE ITEMS in the decomposition. -->
<!-- GUIDANCE: Figure – Example of partitioning of SOFTWARE ITEMS -->
<!-- GUIDANCE: For this example, the MANUFACTURER knows, due to the type of MEDICAL DEVICE SOFTWARE being developed, that the preliminary software safety classification for the SOFTWARE SYSTEM is software safety class C. During software ARCHITECTURE design the MANUFACTURER has decided to partition the SYSTEM, as shown, with 3 SOFTWARE ITEMS – X, W and Z. The MANUFACTURER is able to segregate all SOFTWARE SYSTEM contributions to HAZARDS and HAZARDOUS SITUATIONS which could result in death or SERIOUS INJURY to SOFTWARE ITEM Z and all remaining SOFTWARE SYSTEM contributions to HAZARDS and HAZARDOUS SITUATIONS which could result in a non-SERIOUS INJURY to SOFTWARE ITEM W. SOFTWARE ITEM W is classified as software safety class B and SOFTWARE ITEM Z is at software safety class C. SOFTWARE ITEM Y therefore must be classified as Class C). The SOFTWARE SYSTEM is also at a software safety class C per this requirement. SOFTWARE ITEM X has been classified at a software safety class of A. The MANUFACTURER is able to document a rationale for the segregation between SOFTWARE ITEMS X and Y, as well as SOFTWARE ITEMS W and Z, to assure the integrity of the segregation. If partitioning segregation is not possible between SOFTWARE ITEMS X and Y, then SOFTWARE ITEM X must be classified in software safety class C.> -->

### Mechanical Design

<!-- GUIDANCE: [System Level: Optional: Mechanical design concept can be part of the System Architecture Overview, and details normally apply to Sub-Systems details. If applicable in the system level, a brief of major mechanical design approaches or concepts can be included, with reference to subsequent more detailed Child documents or with pointers to the implementing Sub-Systems. Examples: Air Bearing, Tilt, a new Couch with significantly enhanced performance, faster rotation, Spherical DMS etc.] -->
<!-- GUIDANCE: repeatable example section: <Mechanical Sub-System One> -->
<!-- GUIDANCE: [Include a sub-system overview. If an SSDS is written for the sub-system, include the overview, provide a reference to the SSDS or MeDS, and do not include the detail sections below, as the detail will be in the SSDS or MeDS.] -->
<!-- GUIDANCE: repeatable example section: <Detail or Module One> -->
<!-- GUIDANCE: [Describe details of the sub-system. Describe how the design will address the features, functionality, and performance requirements for the software sub-system.] -->
<!-- GUIDANCE: [As appropriate, perform Make, Buy, or Reuse analyses for critical elements of the design. For example, if there is a choice to develop and/or manufacture a load bearing bracket internally or to have it outsourced, describe the rationale for the decision.] -->
<!-- GUIDANCE: [or:] -->
<!-- GUIDANCE: [If an MeDS is written for the module, include an overview, provide a reference to the MeDS, and do not include the detail, as it will be in the MeDS.] -->
<!-- GUIDANCE: repeatable example section: <Detail or Module Two> -->
<!-- GUIDANCE: [Add additional details or modules, as needed.] -->
<!-- GUIDANCE: repeatable example section: <Mechanical Sub-System Two> -->
<!-- GUIDANCE: [Add additional sub-systems, as needed.] -->

### Interfaces

<!-- GUIDANCE: [System (SDS): Identify the means by which the system/sub-systems interacts with each other and with external entities. Separate subsections or referenced child DHF documents are to be used for each communication protocol, network topology, data coupling or hardware interface. Take care not to duplicate if sufficiently detailed in the System Architecture section. -->
<!-- GUIDANCE: Sub-Systems (SSDS): Identify the means by which modules interact with each other and with external entities. Separate subsections or referenced child DHF documents are to be used for each communication protocol, network topology, data coupling or hardware interface] -->
<!-- GUIDANCE: [A good practice can be to have only a high level mapping and description in this (SDS) document, and to point at DHF “Child” documents that describe the interfaces in details. The Author of this document will decide on the approach per complexity and needed breakdown] -->

### Database and Persistent Data Elements

<!-- GUIDANCE: [System/Sub-System - Define any databases or other data storage design elements including data file formats and management of the data. A good practice can be to have only a high level mapping and description in this (SDS) document, and to point at DHF “Child” documents that describe the design or interfaces in details. The Author of this document will decide on the approach per complexity and needed breakdown] -->

### Error Handling and Reporting

<!-- GUIDANCE: [System/Sub-System: Define any sub-system level design elements for errors and fault conditions. A good practice can be to have only a high level mapping and description in this (SDS) document, and to point at DHF “Child” documents that describe the interfaces in details. The Author of this document will decide on the approach per complexity and needed breakdown] -->

### Third Party Solutions

<!-- GUIDANCE: [Identify any third party solutions (off-the-shelf software / hardware, sub-systems of non-medical origins, etc.) and the design necessary to incorporate those solutions. Note that this should also flow down into lower level design specifications. Each such solution should be uniquely identified (numbered or named) within this specification and referenced within lower level specifications. -->
<!-- GUIDANCE: 3rd Party Solutions are part of the System/Sub-System, and as so their features, performance, interfaces, serviceability, infrastructure within the system/Sub-System, and any other relevant design details are part of the design documents, although their lower level internal design might be unknown to us (e.g. SOUP)] -->
<!-- GUIDANCE: [Notice potential redundancy with Section 4.5, Software Design. Avoid duplicates and use references as needed] -->

### Legacy Elements

<!-- GUIDANCE: [Identify any legacy elements (software or hardware) and design considerations necessary for incorporation. Note that this should also flow down into lower level design specifications. Each such legacy element should be uniquely identified (numbered or named) within this specification and referenced within lower level specifications.] -->
<!-- GUIDANCE: [Notice potential redundancy with Section 4.5 (Software Design) or Section 4.10 (Third Party Solutions). Avoid duplicates and use references as needed Legacy Elements are part of the System/Sub-System, and as so their features, performance, interfaces, serviceability, infrastructure within the system/Sub-System, and any other relevant design details are part of the design documents, although their lower level internal design might be unknown to us, being defined and designed a long time ago. In such cases we may consider these elements as SOUP.] -->

## FUNCTIONAL AND PERFORMANCE SPECIFICATION

<!-- GUIDANCE: [List the functional and performance specifications as described in the design defined above. Consider system/Sub-System level software, hardware, and mechanical interactions as appropriate. Examples of these specifications are: -->
<!-- GUIDANCE: Memory maps -->
<!-- GUIDANCE: Register assignments -->
<!-- GUIDANCE: Power consumption -->
<!-- GUIDANCE: Processing algorithms and throughput -->
<!-- GUIDANCE: Sample and data rates -->
<!-- GUIDANCE: Signal ranges and shaping -->
<!-- GUIDANCE: Timing of critical operations (acquisition and reconstruction with default protocols) -->
<!-- GUIDANCE: Error rates, standard deviations, etc. -->
<!-- GUIDANCE: Sample and data rates -->
<!-- GUIDANCE: Computer architecture and performance -->
<!-- GUIDANCE: Safety considerations / required mitigations per Risk Management File -->
<!-- GUIDANCE: Weight supported -->
<!-- GUIDANCE: Positioning accuracy -->
<!-- GUIDANCE: Speed(s) -->
<!-- GUIDANCE: Acceleration(s) -->
<!-- GUIDANCE: Range(s) of travel -->
<!-- GUIDANCE: Simulators and / or designs for their use -->
<!-- GUIDANCE: Critical components] -->
<!-- GUIDANCE: Note: many of the items above are possibly covered in other chapters. Avoid duplication]. -->

## SAFETY AND REGULATORY

<!-- GUIDANCE: [Describe any design elements to address regulatory concerns such as OSHPD, UL, EMC, etc. This section may also be used to describe how the design addresses Risk Management related requirements if not covered elsewhere. -->
<!-- GUIDANCE: In the SDS only main design guidelines will be provided. Most of the detailed description will naturally be in the SSDS that inherit these requirements from the SRS to the SSRS] -->

## FIELD DEPLOYMENT

### Supported Configurations

<!-- GUIDANCE: [As related to the deployment of the product to the field, define the supported configurations, including peripherals, interaction with other systems, network topologies, etc. -->

### Installation

<!-- GUIDANCE: [System/Sub-Systems: As related to the deployment of the product to the field, define the design elements for both hardware and software installation. Include items such as room configurations, environmental considerations, special shipping and packing, documentation, calibration and adjustment, tools and equipment, installation safety, installation time, user documentation, localization, and training, and performance testing and software.] -->

### Upgrades

<!-- GUIDANCE: [System/Sub-Systems: As related to the deployment of the product to the field, define the design elements to allow upgrading the product.] -->

## MANUFACTURING, SERVICE, AND SUPPORT

<!-- GUIDANCE: [System/Sub-Systems: Identify any diagnostics and other tools in this section, including documentation. Testing access points, diagnostics, test fixtures, calibrations, and etc. are to be addressed. Add additional subsections as appropriate.] -->

### System Calibration and Quality Assurance

<!-- GUIDANCE: [Define the test beds, calibrations, test fixtures, software tools etc. needed to optimize the system and maintain its performance during manufacturing, installation and in the field, including serviceability.] -->

### Design for Manufacturing

<!-- GUIDANCE: [SSDS: Most manufactured parts relate to Sub-Systems. SDS: Certain System approaches and highlights may belong for SDS. SDS/SSDS may point to DHF Child Documents with more detailed design specifications.] -->

### Design for Serviceability

#### System/Sub-System Installation and Upgrade

<!-- GUIDANCE: [Define System & Sub-System installation design elements] -->

#### Remote Service and Monitoring

<!-- GUIDANCE: [Define System & Sub-System design elements for remote connectivity elements, monitoring and data mining for remote export] -->

#### Design for Diagnostics and System Configuration

<!-- GUIDANCE: [Cover in this section subjects such as: -->
<!-- GUIDANCE: Diagnostics & Trouble shooting - Define the design elements for Visual Diagnostics and other troubleshooting tools including remote access. -->
<!-- GUIDANCE: System Logs and Log Viewer(s) - Define the design elements for capturing and managing any persistent logs. -->
<!-- GUIDANCE: System Configuration (Service View) - Define the design elements for capturing System Configuration design.] -->

#### System Maintenance tools and Serviceability supported HW and SW design.

<!-- GUIDANCE: [Define the design elements for capturing System Maintenance tools including remote access and O-Level Access. Point to sub-system hardware design specification for serviceability HW design and serviceability SW requirements] -->

## Design Elements – Requirements Specification Traceability

<!-- GUIDANCE: [This chapter is applicable for SSDS only, and shall be included in SDS only if no SSDS documents exist. If it is not applicable for this document, state N/A. -->
<!-- GUIDANCE: For sub-system main elements (main design output parts, e.g. FRUs), define which sub-system requirements (Design Input) this element is designed to meet] -->
<!-- GUIDANCE: Example for DMS sub-system: -->

| Sub-system Element Name | Element Type | Part Number / Unique Id | SSRS Requirements Id(s) |
| --- | --- | --- | --- |

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

<!-- GUIDANCE: Signatures and dates are captured in PLM Tool as part of the document change order -->

| Signature Reason | Function | Name |
| --- | --- | --- |

<!-- GUIDANCE: APPENDIX <x> – <Title> -->
