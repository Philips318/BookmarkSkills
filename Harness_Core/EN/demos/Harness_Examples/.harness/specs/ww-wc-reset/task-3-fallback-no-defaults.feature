Feature: Fallback when DICOM default Window/Level is unavailable
  As a radiologist viewing a CT image without DICOM windowing tags
  I want the reset function to apply a sensible fallback
  So that I always have usable display settings after a reset

  Background:
    Given the configured fallback Window Width is 400 and Window Center is 40

  @demo
  Scenario: Fallback values applied when DICOM windowing tags are absent
    Given a CT series is loaded without DICOM windowing tags
    And the current Window Width is 800 and Window Center is -200
    When the operator activates the ResetWindowLevelButton
    Then the Window Width is 400
    And the Window Center is 40
    And the confirmation message indicates that fallback defaults were used

  Scenario: Warning is logged when fallback values are used
    Given a CT series is loaded without DICOM windowing tags
    When the operator activates the ResetWindowLevelButton
    Then a warning-level log entry is recorded for the missing DICOM defaults

  Scenario: Fallback applied when DICOM Window Width is zero
    Given a CT series is loaded with a DICOM Window Width of 0
    When the operator activates the ResetWindowLevelButton
    Then the Window Width is 400
    And the Window Center is 40
    And the confirmation message indicates an invalid DICOM value was encountered

  Scenario: Fallback applied when DICOM Window Width is negative
    Given a CT series is loaded with a DICOM Window Width of -100
    When the operator activates the ResetWindowLevelButton
    Then the Window Width is 400
    And the Window Center is 40
