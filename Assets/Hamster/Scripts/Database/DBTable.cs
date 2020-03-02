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
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class DBTable<T> {
  // Abstraction for a table on the database, containing
  // a keyed set of an arbitrary class or structure.
  // Useful for large datasets, where you don't want to have to send
  // the whole table every time a single value changes.  (Changes
  // to a single value will only involve transmitting/receiving that element.
  // Unlike DBStruct, which transmits the entire object whenever any
  // part of it changes.)
  //
  // Note that T needs to be serializable, and should not contain
  // any templated members, or they will fail to render in JSON.
  // See https://docs.unity3d.com/ScriptReference/JsonUtility.ToJson.html
  // for the full limits on what can be serialized.

  public string tableName = "<<UNNAMED TABLE>>";
  public Dictionary<string, DBObj<T>> data { get; private set; }
  public Dictionary<string, T> newData { get; private set; }
  public List<string> deletedEntries { get; private set; }
  public bool areChangesPending { get; private set; }

  private Object clearMutexLock = new Object();
  private Object applyChangeLock = new Object();

  DBTable() {
    DiscardRemoteChanges();
  }

  Firebase.FirebaseApp app;

  public DBTable(string name, Firebase.FirebaseApp app) {
    this.app = app;
    tableName = name;
    data = new Dictionary<string, DBObj<T>>();
    newData = new Dictionary<string, T>();
    deletedEntries = new List<string>();

    addListeners();
  }

  private void addListeners() {
    
  }

  private void removeListeners() {
    
  }

  public void Add(string key, T value) {
    data.Add(key, new DBObj<T>(value));
    data[key].isDirty = true;
  }

  public DBObj<T> this[string key] {
    get {
      if (data.ContainsKey(key))
        return data[key];
      else
        return null;
    }
    set {
      data.Add(key, value);
    }
  }

  // Override this to make custom nonsense like tables that have more
  // than one type in them.
  public virtual T GetFromJson(string json) {
    return JsonUtility.FromJson<T>(json);
  }

  void OnChildAdded() {
    
  }

  void OnChildChanged() {
    
  }

  void OnChildRemoved() {
    
  }

  public void ApplyRemoteChanges() {
    lock (applyChangeLock) {
      if (areChangesPending) {

        foreach (string key in deletedEntries) {
          data.Remove(key);
        }
        foreach (KeyValuePair<string, T> pair in newData) {
          if (!data.ContainsKey(pair.Key)) {
            data[pair.Key] = new DBObj<T>();
          }
          data[pair.Key].data = pair.Value;
        }
        areChangesPending = false;
      }
    }
  }

  public void DiscardRemoteChanges() {
    areChangesPending = false;
  }

  // Returns a guaranteed unique string, usable as a key value.
  public string GetUniqueKey() {
    return null;
  }

  // Clears out the table on the server.
  public void Clear() {
    lock (clearMutexLock) {
      
    }
  }

  public void PushData() {
    lock (applyChangeLock) {
      
    }
  }
}

public class DBObj<T> {
  public bool isDirty = true;
  public T data;

  public DBObj() { }

  public DBObj(T data) {
    this.data = data;
  }
}