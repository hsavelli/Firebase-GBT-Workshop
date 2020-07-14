using System;
using System.Security.Cryptography;
using Firebase.Auth;
using UnityEngine;
using Random = System.Random;

namespace Hamster.States {
  /// <summary>
  /// This utility is meant to throw a random type of exception
  /// to generate a few different types of issues in Crashlytics.
  /// Return a pseudo random exception type from the choices
  /// available. The caller of this utility is responsible
  /// for instantiating/throwing the exception.
  /// </summary>
  public static class PseudoRandomExceptionChooser {
    private static readonly Random RandomGenerator;
    static PseudoRandomExceptionChooser() {
      RandomGenerator = new Random();
    }

    /// <summary>
    /// Throw a random exception from the choices in this directory. Demonstrate
    /// a different set of functions based on which exception is chosen.
    /// </summary>
    /// <param name="message"></param>
    public static void Throw(String message) {
      
      int exceptionIndex = RandomGenerator.Next(0, 6);

      switch (exceptionIndex) {
        case 0:
          Debug.Log("Menu meltdown is imminent.");
          ThrowMenuMeltdown(message);
          break;
        case 1:
          Debug.Log("User triggered another forced exception.");
          ThrowAnotherForcedException(message);
          break;
        case 2:
          Debug.Log("User triggered an intentionally obscure exception.");
          ThrowIntentionallyObscureException();
          break;
        case 3:
          Debug.Log("User triggered a random text exception.");
          ThrowRandomTextException(message);
          break;
        case 4:
          Debug.Log("User triggered an equally statistically likely exception.");
          ThrowStatisticallyAsLikelyException(message);
          break;
        default:
          Debug.Log(String.Format("Could not find index {0} - using default meltdown exception", exceptionIndex));
          ThrowMenuMeltdown(message);
          break;
      }
    }

    private static void ThrowMenuMeltdown(String message) {
      try {
        throw new MenuMeltdownException(message);
      }
      catch (Exception e) {
        Debug.Log(e);
      }
    }

    private static void ThrowAnotherForcedException(String message) {
      try {
        throw new AnotherForcedException(message);
      }
      catch (Exception e) {
        Debug.LogException(e);
      }
    }

    private static void ThrowIntentionallyObscureException() {
      try {
        throw new IntentionallyObscureException("An error occurred.");
      }
      catch (Exception e) {
        Debug.LogException(e);
      }
    }

    private static void ThrowRandomTextException(String message) {
      try {
        throw new RandomTextException(message);
      }
      catch (Exception e) {
        Debug.Log(e);
      }
    }

    private static void ThrowStatisticallyAsLikelyException(String message) {
      try {
        throw new StatisticallyAsLikelyException(message);
      }
      catch (Exception e) {
        Debug.Log(e);
      }
    }


  }
}