// Copyright 2017 Google Inc. All rights reserved.
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

using UnityEngine;
using System.Collections.Generic;
using System;

namespace Hamster.States {

    // State used to upload the time taken to beat the current level to the database.
    class UploadTime : BaseState
    {
        // The time that will be saved.
        public long Time { get; private set; }

        // Whether the time was uploaded, used during cleanup.
        private bool TimeUploaded { get; set; }

    }
}
