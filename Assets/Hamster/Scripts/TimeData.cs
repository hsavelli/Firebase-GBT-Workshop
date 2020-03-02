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

using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Hamster {
  public class TimeDataUtil {
    // Maximum rank records to download
    const int Max_Rank_Records = 5;

    // The root folder to store replay data.
    // The replay data are stored in {Storage_Replay_Root_Folder}/{MapType}/{MapID}/{RecordID}
    const string Storage_Replay_Root_Folder = "Replay/";

    // The postfix of the top rank record for each map
    const string Database_Ranks_Postfix = "Top/Ranks/";

    // The postfix of the top shared replay record for each map
    const string Database_Replays_Postfix = "Top/SharedReplays/";

    // Property name of time stored in the database.  Stored in milliseconds.
    const string Database_Property_Time = "score";

    // Property name of player display name stored in the database
    const string Database_Property_Name = "name";

    // Property name of storage path to replay record stored in the database
    const string Database_Property_ReplayPath = "replayPath";

    // Name of the property to determine whether the replay record is shared
    const string Database_Property_IsShared = "isShared";

    // Configuration of key, filename and paths to upload time record and replay data
    private struct UploadConfig {
      public string key;
      public string storagePath;
      public string dbRankPath;
      public string dbSharedReplayPath;
      public bool shareReplay;
    }

    // Uploads the time data to the database, and returns the current top time list.
    public static void UploadReplay(
        long time,
        LevelMap map,
        ReplayData replay) {
      
    }

    // Gets the path for the top ranks on the database, given the level's database path
    // and map id.
    public static string GetDBRankPath(LevelMap map) {
      return "Leaderboard/Map/" + GetPath(map) + Database_Ranks_Postfix;
    }

    // Gets the path, given the level's database path and map id.
    // This path is used for both database and storage.
    private static string GetPath(LevelMap map) {
      if (!string.IsNullOrEmpty(map.DatabasePath)) {
        return map.DatabasePath + "/";
      } else {
        return "OfflineMaps/" + map.mapId + "/";
      }
    }

    // Gets the path for the top shared replays on the database, given the level's database path
    // and map id.  Note that not everyone wants to share replay record
    private static string GetDBSharedReplayPath(LevelMap map) {
      return "Leaderboard/Map/" + GetPath(map) + Database_Replays_Postfix;
    }

    // Returns the top times, given the level's database path and map id.
    // Upload the replay data to Firebase Storage
    private static Task<StorageMetadata> UploadReplayData() {
      return null;
    }

    // Utility function to get the top shared replay record storage path from the database
    public static Task<string> GetBestSharedReplayPathAsync(LevelMap map) {
            return null;
    }
  }
}
