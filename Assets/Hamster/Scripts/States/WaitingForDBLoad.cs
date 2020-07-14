﻿// Copyright 2017 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using UnityEngine;

namespace Hamster.States {
  // Utility state, for fetching structures from the database.
  // Returns the result in the result struct.
  class WaitingForDBLoad<T> : BaseState where T : class {

    const float TimeoutSeconds = 10.0f;

    T result = default(T);
    string path;
    bool isComplete = false;
    bool wasSuccessful = false;
    Menus.SingleLabelGUI menuComponent;
    float timeoutTime;

    public WaitingForDBLoad(string path) {
      this.path = path;
    }

    // Initialization method.  Called after the state
    // is added to the stack.
    public override void Initialize() {
      menuComponent = SpawnUI<Menus.SingleLabelGUI>(StringConstants.PrefabsSingleLabelMenu);
      timeoutTime = Time.realtimeSinceStartup + TimeoutSeconds;
    }

    // Called once per frame when the state is active.
    public override void Update() {
      if (isComplete || Time.realtimeSinceStartup > timeoutTime) {
        manager.PopState();
      } else {
        UpdateLabelText();
      }
    }

    protected virtual T ParseResult(string json) {
      return JsonUtility.FromJson<T>(json);
    }

    void UpdateLabelText() {
      if (menuComponent != null) {
        menuComponent.LabelText.text =
          StringConstants.LabelLoading + Utilities.StringHelper.CycleDots();
      }
    }

    public override StateExitValue Cleanup() {
      DestroyUI();
      return new StateExitValue(
        typeof(WaitingForDBLoad<T>), new Results(path, result, wasSuccessful));
    }

    // Class for encapsulating the results of the database load, as
    // well as information about whether the load was successful
    // or not.
    public class Results {
      public string path;
      public T results;
      public bool wasSuccessful;

      public Results(string path, T results, bool wasSuccessful) {
        this.path = path;
        this.results = results;
        this.wasSuccessful = wasSuccessful;
      }
    }

  }
}
