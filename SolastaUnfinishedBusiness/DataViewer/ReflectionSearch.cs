using System;
using System.Collections;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Displays;
using UnityEngine;
using static SolastaUnfinishedBusiness.Api.LanguageExtensions.StringExtensions;

namespace SolastaUnfinishedBusiness.DataViewer;

/**
 * * Strategy For Async Deep Search
 * *
 * * --- update
 * *
 * * duh can't do real async/Task need to use unity coroutines ala https://docs.unity3d.com/ScriptReference/MonoBehaviour.StartCoroutine.html
 * *
 * * two coroutines implemented with Task() and async/await
 * *      Render Path - regular OnGUI on the main thread
 * *      Search Loop
 * *          background thread posting updates using IProgress on the main thread using something like
 * *              private async void Button_Click(object sender, EventArgs e)
 * *              here https://stackoverflow.com/questions/12414601/async-await-vs-backgroundworker
 * *
 * * Store Node.searchText as a static
 * *
 * * Add to node
 * *      HashSet
 * String
 * matches
 * *      searchText
 * *
 * * Node.Render(depth) - main thread (UI)
 * *      if (!autoExpandKeys.IsEmpty), foreach (key, value) display {key}, {value | Render(children+1) )
 * *      if (isExpanded) foreach (key, value) display {key}, {value | Render(children+1) )
 * *     yield
 * *
 * * Node.Search(string[] keyPath, Func
 * Node, Bool
 * matches, int depth) - background thread
 * *      autoMatchKeys.Clear()
 * *      foreach (key, value)
 * *          if (matches(key) matches += key
 * *          if (value.isAtomic && matches(value))  matches += key
 * *          if we added any keys to matches then {
 * *              foreach parent = Node.parent until Node.IsRoot {
 * *                  depth -= 1
 * *                  parKey = keyPath[depth]
 * *                  if parent.autoMatchKeys.Contains(parKey) done // another branch populated through that key
 * *                  parent.matches += parKey
 * *              }
 * *          }
 * *          else (value as Node).Search(keyPath + key, matches)
 * *
 * *
 * * Bool Matches(text)
 * *      if (text.contains(searchText) return true
 * *
 * * On User click expand for Node, Node.isExpanded = !Node.isExpanded
 * *
 * * On searchText change
 * *      foreach Node in Tree, this.matches.Clear()
 * *
 */
internal class ReflectionSearch : MonoBehaviour
{
    private static readonly HashSet<int> VisitedInstanceIDs = new();
    private static ReflectionSearch _shared;
    private IEnumerator _searchCoroutine;

    internal bool IsSearching => _searchCoroutine != null;
    private static int SequenceNumber { get; set; }

    internal static ReflectionSearch Shared
    {
        get
        {
            if (_shared != null)
            {
                return _shared;
            }

            _shared = new GameObject().AddComponent<ReflectionSearch>();
            DontDestroyOnLoad(_shared.gameObject);

            return _shared;
        }
    }

    internal void StartSearch(Node node, string searchText, SearchProgress updater, ReflectionSearchResult resultRoot)
    {
        if (_searchCoroutine != null)
        {
            StopCoroutine(_searchCoroutine);
            _searchCoroutine = null;
        }

        VisitedInstanceIDs.Clear();
        resultRoot.Clear();
        resultRoot.Node = node;
        StopAllCoroutines();
        updater(0, 0, 1);

        if (node == null)
        {
            return;
        }

        SequenceNumber++;
        // ReSharper disable once InvocationIsSkipped
        Main.Log($"seq: {SequenceNumber} - search for: {searchText}");

        if (searchText.Length == 0)
        {
            return;
        }

        var todo = new List<Node> { node };

        _searchCoroutine = Search(searchText, todo, 0, 0, SequenceNumber, updater, resultRoot);
        StartCoroutine(_searchCoroutine);
    }

    internal void Stop()
    {
        if (_searchCoroutine != null)
        {
            StopCoroutine(_searchCoroutine);
            _searchCoroutine = null;
        }

        StopAllCoroutines();
    }

    private IEnumerator Search(
        string searchText,
        List<Node> todo,
        int depth,
        int visitCount,
        int sequenceNumber,
        SearchProgress updater,
        ReflectionSearchResult resultRoot)
    {
        yield return null;

        if (sequenceNumber != SequenceNumber)
        {
            yield return null;
        }

        var newTodo = new List<Node>();
        var breadth = todo.Count;

        foreach (var node in todo)
        {
            var foundMatch = false;
            var instanceID = node.InstanceID;
            var alreadyVisited = false;

            if (instanceID is { } instID)
            {
                if (!VisitedInstanceIDs.Add(instID))
                {
                    alreadyVisited = true;
                }
            }

            visitCount++;

            try
            {
                if (node.Name.Matches(searchText) || node.ValueText.Matches(searchText))
                {
                    foundMatch = true;
                    updater(visitCount, depth, breadth);
                    resultRoot.AddSearchResult(node);
                    // ReSharper disable once InvocationIsSkipped
                    Main.Log($"{depth} matched: {node.GetPath()} - {node.ValueText}");
                    // ReSharper disable once InvocationIsSkipped
                    Main.Log($"{resultRoot}");
                }
            }
            catch (Exception e)
            {
                // ReSharper disable once InvocationIsSkipped
                Main.Log($"{depth} caught - {e}");
            }

#pragma warning disable CS0618 // Type or member is obsolete
            node.Matches = foundMatch;
#pragma warning restore CS0618 // Type or member is obsolete

            if (!foundMatch && visitCount % 100 == 0)
            {
                updater(visitCount, depth, breadth);
            }

            if (node.HasChildren && !alreadyVisited)
            {
                if (node.InstanceID is { } instID2 && instID2 == GetInstanceID())
                {
                    break;
                }

                if (node.Name == "searchCoroutine")
                {
                    break;
                }

                try
                {
                    newTodo.AddRange(node.GetItemNodes());
                }
                catch (Exception e)
                {
                    // ReSharper disable once InvocationIsSkipped
                    Main.Log($"{depth} caught - {e}");
                }

                try
                {
                    newTodo.AddRange(node.GetComponentNodes());
                }
                catch (Exception e)
                {
                    // ReSharper disable once InvocationIsSkipped
                    Main.Log($"{depth} caught - {e}");
                }

                try
                {
                    newTodo.AddRange(node.GetPropertyNodes());
                }
                catch (Exception e)
                {
                    // ReSharper disable once InvocationIsSkipped
                    Main.Log($"{depth} caught - {e}");
                }

                try
                {
                    newTodo.AddRange(node.GetFieldNodes());
                }
                catch (Exception e)
                {
                    // ReSharper disable once InvocationIsSkipped
                    Main.Log($"{depth} caught - {e}");
                }
            }

            if (visitCount % 1000 == 0)
            {
                yield return null;
            }
        }

        if (newTodo.Count > 0 && depth < GameServicesDisplay.MaxSearchDepth)
        {
            yield return Search(searchText, newTodo, depth + 1, visitCount, sequenceNumber, updater, resultRoot);
        }
        else
        {
            Stop();
        }
    }

    internal delegate void SearchProgress(int visitCount, int depth, int breadth);
}
