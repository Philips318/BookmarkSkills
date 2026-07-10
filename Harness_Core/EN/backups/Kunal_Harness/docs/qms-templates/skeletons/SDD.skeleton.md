# Software Design Document

## PURPOSE

<!-- GUIDANCE: <This short paragraph provides a simple and brief statement of what the software item does. It is NOT a statement about what a design document is—in other words, it shall not state, “This document identifies the design of such and such software item.”> -->
<!-- GUIDANCE: [Example: The CANopen Software Test Framework ("Framework") provides a foundation of common functionality used by all simple axis developers to develop CANopen method call test stubs. These test stubs enable the developers to test their axis software without the presence of HW, e.g., SIB, CAN bus, axis devices, etc.] -->

## SCOPE

<!-- GUIDANCE: This record applies to the Philips CT/AMI. -->
<!-- GUIDANCE: [Where applicable, enter information regarding, the audience or organizational entities responsible for implementing the document, any exclusions that aid in understanding the scope, if applicable.] -->

## Overview

<!-- GUIDANCE: <A short history, background, and use of the software item will be presented here. Include a description of how the software item fits in the product and, as applicable, a reference to the parent SDS or SSDS.> -->
<!-- GUIDANCE: [Example: The Scalable Gantry Architecture consists of the SGA application and axis software, as identified by the SGA SSDS (DHF173114). Software stubs and simulators are used to implement and test the application and axis software—both with and without the physical hardware of an axis. Four levels are considered at which to simulate an axis, from the Simple Axis Stubs to CAN Bus Simulators, as shown in the following diagram: -->
<!-- GUIDANCE: Simple Axis Stubs facilitate limited testing of the SGA application down to the simple axis interface; this method is used during early SGA development before any hardware and axis software is available. CANopen SW Stubs extend this testability to include the simple axis software. CAN I/O Driver Stubs will not be used. CAN Bus Simulators extend testability to include the CANopen Stack, SIB & CAN driver, and SIB hardware, facilitating testing of the complete Gantry PC and all of its software as a complete production unit. -->

#### Per IEC 62304 – Section 5.4.1- mandatory for

<!-- GUIDANCE: Class B:Non-serious injury is possible -->
<!-- GUIDANCE: Class C:Death or serious injury is possible -->
<!-- GUIDANCE: ] -->
<!-- GUIDANCE: [This section describes the top-level design of the software item including the theory of operation. If describing a software item that is not a unit, include the decomposition into software units and the relationships between the software units. -->
<!-- GUIDANCE: The design overview describes the big picture to help with understanding the details that will follow. A diagram can help explain the overall design and relation between items or other design Modules. The content and method of presentation must be helpful to the engineers maintaining this design as well as the intended target audience. -->
<!-- GUIDANCE: The design overview or subsequent sections shall include identification of software of unknown providence (SOUP, e.g., off-the-shelf software / hardware, subsystems of non-medical origins, etc.) as well as identification of legacy Modules (software or hardware) and the design necessary to incorporate those solutions. To the extent that related identifiers exist in upper level design or requirements, those identifiers will be referenced here. If an actual supplier or component is known, it shall be identified in this document.] -->

## Architecture Views

<!-- GUIDANCE: <Describe the architectural views relevant to clarify the design of <Subject> and to provide guidance for detailing. -->
<!-- GUIDANCE: The following are examples for architecture views to be considered, as applicable, when specifying requirements (this is not an exhaustive list of possibilities): -->
<!-- GUIDANCE: Functional View -->
<!-- GUIDANCE: Interface View -->
<!-- GUIDANCE: Physical View -->
<!-- GUIDANCE: Deployment View -->
<!-- GUIDANCE: Hardware View -->
<!-- GUIDANCE: Usage View -->
<!-- GUIDANCE: Network View -->
<!-- GUIDANCE: Interaction View -->
<!-- GUIDANCE: Communication/process View -->
<!-- GUIDANCE: Code Distribution View -->
<!-- GUIDANCE: Events View -->
<!-- GUIDANCE: Data Storage View -->
<!-- GUIDANCE: Segregation View -->
<!-- GUIDANCE: Add for each architecture view in scope a subsection to detail the view. -->
<!-- GUIDANCE: Segregation: Identify any segregation between software items that is necessary for risk control and state how to ensure that such segregation is effective. An example of segregation is to have software items execute on different processors. The effectiveness of the segregation can then be ensured by having no shared resources between the processors. Other means of segregation can also be applied, when effectiveness can be described by the software architecture. -->
<!-- GUIDANCE: > -->

### Software Architecture View

<!-- GUIDANCE: <Describe the SW architecture and add a block diagram of the SW modules and the connection between them> -->
<!-- GUIDANCE: repeatable example section: <View 2> -->
<!-- GUIDANCE: [Describe the SW architecture in additional view, if needed] -->

## Design Details

<!-- GUIDANCE: <Provide, as starting point for design teams, for the Module the required functionality, technical choices (hardware / software /mechanical / electrical) and other design constraints. -->
<!-- GUIDANCE: In case of a module breakdown, provide for each Module and interface in the Module: the required functionality, technical choices (hardware / software /mechanical / electrical) and other design constraints. In this case, create in the section 4.2. Module detailed design space for each Module a separate section, describing the functionality and design constraints. In case -->
<!-- GUIDANCE: Describe if needed, also the detailed design space for parts in the Module. -->
<!-- GUIDANCE: > -->
<!-- GUIDANCE: < Traceability from design output to design input requirements: In each section, include a reference to SwRS (e.g. to a relevant SwRS section, to relevant requirements ids) > -->

### Allocation of SW Quality Aspects

<!-- GUIDANCE: <Allocate quality aspects requirements over the Module -->
<!-- GUIDANCE: The following are examples to be considered, as applicable, when describing the design (this is not an exhaustive list of possibilities): -->
<!-- GUIDANCE: Performance -->
<!-- GUIDANCE: Reliability -->
<!-- GUIDANCE: Memory Usage -->
<!-- GUIDANCE: > -->

### Detailed Module Design

<!-- GUIDANCE: <Describe the detailed design space for the Module. In case of a module breakdown, provide for each Module the detailed design space. -->
<!-- GUIDANCE: The following are examples to be considered, as applicable, when describing the design (this is not an exhaustive list of possibilities): -->
<!-- GUIDANCE: COTS -->
<!-- GUIDANCE: Start up -->
<!-- GUIDANCE: Processing execution models -->
<!-- GUIDANCE: Memory management -->
<!-- GUIDANCE: Logging, retrieving, and storing data -->
<!-- GUIDANCE: Exception handling -->
<!-- GUIDANCE: Interrupt handling -->
<!-- GUIDANCE: Self-test -->
<!-- GUIDANCE: State machines -->
<!-- GUIDANCE: Algorithms -->
<!-- GUIDANCE: User authentication -->
<!-- GUIDANCE: Security -->
<!-- GUIDANCE: SOUP -->
<!-- GUIDANCE: Serviceability (e.g. diagnostics, logging) -->
<!-- GUIDANCE: If only one Module is to be described delete in this section all the subsection titles <Module x> -->
<!-- GUIDANCE: > -->
<!-- GUIDANCE: repeatable example section: <Module 1> -->
<!-- GUIDANCE: [Per IEC 62304 – Section 5.4.1- mandatory for: -->
<!-- GUIDANCE: Class B: Non-serious injury is possible -->
<!-- GUIDANCE: Class C: Death or serious injury is possible] -->

##### Functionality

<!-- GUIDANCE: <Describe the functionality of the module> -->

##### Use Case(s)

<!-- GUIDANCE: <Draw all uses cases relevant for the module> -->
<!-- GUIDANCE: repeatable example section: <Module 1> Design Details -->
<!-- GUIDANCE: <Describe and justify for the module the technical design approach, constraints and any design issue> -->
<!-- GUIDANCE: [IEC 62304 – Section 5.4.2: Class C] -->
<!-- GUIDANCE: [The manufacturer shall develop and document a detailed design for each software unit of the software item.] -->
<!-- GUIDANCE: [Note that this section shall address: -->
<!-- GUIDANCE: Performance related design -->
<!-- GUIDANCE: Equations and algorithm utilized by SW (reference to external algorithm spec is possible) -->
<!-- GUIDANCE: If applicable, Variables definitions and where used] -->

##### Class Diagram

<!-- GUIDANCE: <Draw class diagram of the module and its connection to other modules (if any)> -->

##### Sequence Diagram

<!-- GUIDANCE: <Draw sequence diagrams for any use case and for module startup\power-up> -->

##### SW Safety Classification

<!-- GUIDANCE: [Identify the safety classification of the software item. If the SWDS is describing a feature, state that this section is not applicable for features] -->
<!-- GUIDANCE: [IEC 62304 – Section 4.3.a:] -->
<!-- GUIDANCE: [The software safety classes shall initially be assigned based on severity as follows: -->
<!-- GUIDANCE: Class A: No injury or damage to health possible -->
<!-- GUIDANCE: Class B: Non-serious injury is possible -->
<!-- GUIDANCE: Class C: Death or serious injury is possible] -->
<!-- GUIDANCE: [Example: This is a Class C software item.] -->
<!-- GUIDANCE: [If the SWDS consist of more than one SW module, write the safety classification in the module itself. For example: -->
<!-- GUIDANCE: The SW Safety Classification is detailed in any module that is being detailed below, in chapter 5.4, in this document (as 5.4.x.6).] -->
<!-- GUIDANCE: repeatable example section: <Module n> -->
<!-- GUIDANCE: [Structure is as in section Module 1] -->

#### SOUP Items

<!-- GUIDANCE: [SOUP indicates Software of Unknown Provenance as per the IEC 62304 terminology. It includes all third party solutions and legacy solutions used by the software item being described here.] -->

##### Third Party Solutions

<!-- GUIDANCE: [Identify any third party solutions (off-the-shelf software / hardware, subsystems of non-medical origins, etc.) and the design necessary to incorporate those solutions. Note that this shall also flow down into lower level design specifications. Each such solution shall be uniquely identified (numbered or named) within this specification and referenced within lower level specifications. -->
<!-- GUIDANCE: Identify any solutions provided by PHILIPS groups that do not follow the QMS and the design necessary to incorporate these solutions as a new subsection, if applicable. -->
<!-- GUIDANCE: Per IEC 62304 – Section 5.3.3, 5.3.4 – mandatory for: -->
<!-- GUIDANCE: Class B: Non-serious injury is possible -->
<!-- GUIDANCE: Class C: Death or serious injury is possible] -->
<!-- GUIDANCE: repeatable example section: <Third Party Solution 1> -->

####### Functional & Performance Design

<!-- GUIDANCE: [List the functional & performance requirements for the third party solution] -->

####### Integration design

<!-- GUIDANCE: [Describe the design necessary to integrate the third party tool. Mention relevant interfaces and error handling mechanisms] -->
<!-- GUIDANCE: repeatable example section: <Third Party Solution 2> -->
<!-- GUIDANCE: [Add additional third party solutions] -->

##### Legacy Modules

<!-- GUIDANCE: [Identify any legacy Modules (software or hardware) and design considerations necessary for incorporation. Note that this shall also flow down into lower level design specifications. Each such legacy Module shall be uniquely identified (numbered or named) within this specification and referenced within lower level specifications.] -->

### Interfaces Design

<!-- GUIDANCE: <In case of a module breakdown, describe the detailed design space for each interface in the Module> -->
<!-- GUIDANCE: [The following are examples to be considered, as applicable, when describing the interfaces (this is not an exhaustive list of possibilities): protocol, deployment. Otherwise, delete this section.] -->
<!-- GUIDANCE: [Per IEC 62304 – Section 5.4.3 – mandatory for: Class C: Death or serious injury is possible] -->
<!-- GUIDANCE: [Identify the means by which the software item interacts with external entities, such as APIs, pipes, RPC methods, communication protocols, network topologies, or hardware interfaces. Reference HDD as applicable; use diagrams as applicable. -->
<!-- GUIDANCE: Also, describe all public interfaces exposed by the software item.] -->

### Testing Design

<!-- GUIDANCE: [Per IEC 62304 – Section 5.4.4 –mandatory for: Class C: Death or serious injury is possible] -->

#### Test Overview

<!-- GUIDANCE: [This section is used to provide information that may be useful to a test developer. This may include suggestions for test implementation, identification of key features to be tested, and design details that may be of particular interest to test developers] -->
<!-- GUIDANCE: [The test overview describes the big picture to help with understanding the details that will follow. A diagram can help explain the approach or overall design and relation between modules or other test Modules. The content and method of presentation must be helpful to the engineers maintaining this test as well as the intended target audience.] -->

#### Primary Test Considerations and Design

<!-- GUIDANCE: [Reference the “Design” and “Implementation Specifications” sections as applicable. -->
<!-- GUIDANCE: Describe here how the SW will be tested, e.g. using Simulator that simulates one of the external component of the system.] -->

### Design Environments

<!-- GUIDANCE: <This section is used to provide information about the developing environment> -->
<!-- GUIDANCE: [The following are examples to be considered, as applicable, when describing the developing environment (this is not an exhaustive list of possibilities): IDE (Visual Studio, Eclipse), programming languages (C#, C++), intended OS, etc.] -->

#### SW development tool(s)

<!-- GUIDANCE: <Describe the version of each tool used for developing the SW, for example: Microsoft Visual Studio 2013 with dot NET framework 4.6.2> -->

#### Programming language(s)

<!-- GUIDANCE: <Describe the programming language that is used to develop the SW, for example: C#, Java, etc.> -->

#### Operating System

<!-- GUIDANCE: <Describe the operating system on which the SW will be executed, For example: Windows 7 64-bit and above, Ubuntu 16.04 64bit.> -->

### Security Features

<!-- GUIDANCE: <Describe here the security features including those that are not addressed in the SW requirements> -->
<!-- GUIDANCE: [Example: All security features are inherited from the Console SW as we run this SW on the Host PC owned by the Console team] -->

## TERMS AND ABBREVIATIONS

| Term / Abbreviation | Description |
| --- | --- |

## APPENDICIES

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

| Signature Reason | Function | Name |
| --- | --- | --- |

<!-- GUIDANCE: repeatable example section: APPENDIX <x> – <Title> -->
