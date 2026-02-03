# State Machine Module

A lightweight Finite State Machine (FSM) implementation for character controllers or logic systems.

## Core Components

### `StateMachine`
The base class that manages states and transitions.

### `StateMachineController`
Helper structure for managing state execution order.

### `IStateMachine`
Interface for objects that utilize this state machine.

## Usage

1.  Inherit from `StateMachine` in your specific logic class (e.g., `PlayerStateMachine`).
2.  Define states (enums or classes).
3.  Implement `OnEnter`, `OnUpdate`, `OnExit` logic for each state.
