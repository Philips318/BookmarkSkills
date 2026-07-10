Feature: Reset Window/Level to DICOM default
  As a radiologist viewing a CT image
  I want to reset the Window Width and Window Center to the DICOM default values
  So that I can quickly return to the scanner-recommended display settings

  Background:
    Given a CT series is loaded with DICOM default WW 1500 and WC -600

  @demo
  Scenario: Reset restores DICOM default Window Width and Window Center
    Given the current Window Width is 800 and Window Center is -200
    When the operator activates the ResetWindowLevelButton
    Then the Window Width is 1500
    And the Window Center is -600
    And a confirmation message is displayed in the status bar

  Scenario: Reset button is disabled when no series is loaded
    Given no CT series is loaded in the active viewport
    When the operator views the image display panel
    Then the ResetWindowLevelButton is disabled

  Scenario: Confirmation message includes the DICOM preset name
    Given the CT series includes a preset name "Lung" for the default window
    And the current Window Width is 800 and Window Center is -200
    When the operator activates the ResetWindowLevelButton
    Then the confirmation message includes the text "Lung"

  Scenario: Confirmation message auto-dismisses
    Given the current Window Width is 800 and Window Center is -200
    When the operator activates the ResetWindowLevelButton
    Then a confirmation message is displayed in the status bar
    And the confirmation message disappears automatically within 3 seconds

  Scenario: Keyboard shortcut triggers reset
    Given the current Window Width is 800 and Window Center is -200
    When the operator presses Ctrl+Shift+W
    Then the Window Width is 1500
    And the Window Center is -600
