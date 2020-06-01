Update 1.0.2 will no longer support CustomStates, you should use Actions instead.

For example:
- for the vAIChaseState use the Action 'Chase Target' instead
- for the vAICombatState use the Action 'SimpleCombat' instead
- for the vAISimpleCombatState use the Action 'MeleeCombat' instead
- for the vAIShooterCombatState use the Action 'ShooterCombat' instead

This scripts will still be on the package to avoid breaking any FSM created before.