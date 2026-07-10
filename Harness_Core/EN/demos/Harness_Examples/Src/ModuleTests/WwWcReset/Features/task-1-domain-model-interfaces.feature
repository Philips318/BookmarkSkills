Feature: Domain model and interface contracts for WW/WC Reset
  As a developer building the WW/WC Reset feature
  I want validated domain types and DICOM tag constants
  So that the reset feature has a safe, testable foundation with WW > 0 enforced

  Scenario: Valid Window Width and Center create a WindowLevel
    Given a Window Width of 400 and a Window Center of 40
    When a WindowLevel is created with those values
    Then the WindowLevel has Width 400 and Center 40

  Scenario: Zero Window Width is rejected
    Given a Window Width of 0 and a Window Center of 40
    When a WindowLevel is created with those values
    Then the creation fails with a validation error

  Scenario: Negative Window Width is rejected
    Given a Window Width of -100 and a Window Center of 40
    When a WindowLevel is created with those values
    Then the creation fails with a validation error

  Scenario: WindowLevel preserves the DICOM preset name
    Given a Window Width of 400 and a Window Center of 40 with preset name "Soft Tissue"
    When a WindowLevel is created with those values and preset name
    Then the WindowLevel preset name is "Soft Tissue"

  Scenario: DICOM display tag constants map to correct tag numbers
    Given the DICOM display tag constants
    Then the WindowCenter constant maps to DICOM tag 0028,1050
    And the WindowWidth constant maps to DICOM tag 0028,1051
    And the WindowCenterWidthExplanation constant maps to DICOM tag 0028,1055
