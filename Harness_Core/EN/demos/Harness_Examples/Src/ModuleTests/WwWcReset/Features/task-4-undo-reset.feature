Feature: Undo support for Window/Level reset
  As a radiologist
  I want to undo a Window/Level reset
  So that I can return to my previous custom settings if the reset was unintended

  Background:
    Given a CT series is loaded with DICOM default WW 1500 and WC -600

  @demo
  Scenario: Undo restores the previous Window Width and Window Center
    Given the current Window Width is 800 and Window Center is -200
    And the operator has activated the ResetWindowLevelButton
    When the operator invokes Undo
    Then the Window Width is 800
    And the Window Center is -200

  Scenario: Redo reapplies the reset after an undo
    Given the current Window Width is 800 and Window Center is -200
    And the operator has activated the ResetWindowLevelButton
    And the operator has invoked Undo
    When the operator invokes Redo
    Then the Window Width is 1500
    And the Window Center is -600

  Scenario: Undo is unavailable when no reset has been performed
    Given no Window/Level changes have been made
    When the operator checks the Undo availability
    Then the Undo function for Window/Level changes is not available
