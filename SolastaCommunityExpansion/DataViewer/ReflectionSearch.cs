using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SolastaCommunityExpansion.Api.Infrastructure.StringExtensions;

namespace SolastaCommunityExpansion.DataViewer;

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
     * <String>
     *     matches
     *     *      searchText
     *     *
     *     * Node.Render(depth) - main thread (UI)
     *     *      if (!autoExpandKeys.IsEmpty), foreach (key, value) display {key}, {value | Render(children+1) )
     *     *      if (isExpanded) foreach (key, value) display {key}, {value | Render(children+1) )
     *     *     yield
     *     *
     *     * Node.Search(string[] keyPath, Func
     *     <Node, Bool>
     *         matches, int depth) - background thread
     *         *      autoMatchKeys.Clear()
     *         *      foreach (key, value)
     *         *          if (matches(key) matches += key
     *         *          if (value.isAtomic && matches(value))  matches += key
     *         *          if we added any keys to matches then {
     *         *              foreach parent = Node.parent until Node.IsRoot {
     *         *                  depth -= 1
     *         *                  parKey = keyPath[depth]
     *         *                  if parent.autoMatchKeys.Contains(parKey) done // another branch populated through that key
     *         *                  parent.matches += parKey
     *         *              }
     *         *          }
     *         *          else (value as Node).Search(keyPath + key, matches)
     *         *
     *         *
     *         * Bool Matches(text)
     *         *      if (text.contains(searchText) return true
     *         *
     *         * On User click expand for Node, Node.isExpanded = !Node.isExpanded
     *         *
     *         * On searchText change
     *         *      foreach Node in Tree, this.matches.Clear()
     *         *
     */
public class ReflectionSearch : MonoBehaviour
{
    public delegate void SearchProgress(int visitCount, int depth, int breadth);

    private static readonly HashSet<int> VisitedInstanceIDs = new();
    private static ReflectionSearch _shared;
    private IEnumerator searchCoroutine;

    public bool IsSearching => searchCoroutine != null;
    public static int SequenceNumber { get; private set; }

    public static ReflectionSearch Shared
    {
        get
        {
            if (_shared == null)
            {
                _shared = new GameObject().AddComponent<ReflectionSearch>();
                DontDestroyOnLoad(_shared.gameObject);
            }

            return _shared;
        }
    }

    public void StartSearch(Node node, string searchText, SearchProgress updator, ReflectionSearchResult resultRoot)
    {
        if (searchCoroutine != null)
        {
            StopCoroutine(searchCoroutine);
            searchCoroutine = null;
        }

        VisitedInstanceIDs.Clear();
        resultRoot.Clear();
        resultRoot.Node = node;
        StopAllCoroutines();
        updator(0, 0, 1);
        if (node == null)
        {
            return;
        }

        SequenceNumber++;
        Main.Log($"seq: {SequenceNumber} - search for: {searchText}");
        if (searchText.Length != 0)
        {
            var todo = new List<Node> {node};

            searchCoroutine = Search(searchText, todo, 0, 0, SequenceNumber, updator, resultRoot);
            StartCoroutine(searchCoroutine);
        }
    }

    public void Stop()
    {
        if (searchCoroutine != null)
        {
            StopCoroutine(searchCoroutine);
            searchCoroutine = null;
        }

        StopAllCoroutines();
    }

    private IEnumerator Search(string searchText, List<Node> todo, int depth, int visitCount, int sequenceNumber,
        SearchProgress updator, ReflectionSearchResult resultRoot)
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
            var alreadyVisted = false;
            if (instanceID is int instID)
            {
                if (VisitedInstanceIDs.Contains(instID))
                {
                    alreadyVisted = true;
                }
                else
                {
                    VisitedInstanceIDs.Add(instID);
                }
            }

            visitCount++;

            try
            {
                if (Matches(node.Name, searchText) || Matches(node.ValueText, searchText))
                {
                    foundMatch = true;
                    updator(visitCount, depth, breadth);
                    resultRoot.AddSearchResult(node);
                    Main.Log($"{depth} matched: {node.GetPath()} - {node.ValueText}");
                    Main.Log($"{resultRoot}");
                }
            }
            catch (Exception e)
            {
                Main.Log($"{depth} caught - {e}");
            }
#pragma warning disable CS0618 // Type or member is obsolete
            node.Matches = foundMatch;
#pragma warning restore CS0618 // Type or member is obsolete
            if (!foundMatch && visitCount % 100 == 0)
            {
                updator(visitCount, depth, breadth);
            }

            if (node.hasChildren && !alreadyVisted)
            {
                if (node.InstanceID is int instID2 && instID2 == GetInstanceID())
                {
                    break;
                }

                if (node.Name == "searchCoroutine")
                {
                    break;
                }

                try
                {
                    foreach (var child in node.GetItemNodes())
                    {
                        newTodo.Add(child);
                    }
                }
                catch (Exception e)
                {
                    Main.Log($"{depth} caught - {e}");
                }

                try
                {
                    foreach (var child in node.GetComponentNodes())
                    {
                        newTodo.Add(child);
                    }
                }
                catch (Exception e)
                {
                    Main.Log($"{depth} caught - {e}");
                }

                try
                {
                    foreach (var child in node.GetPropertyNodes())
                    {
                        newTodo.Add(child);
                    }
                }
                catch (Exception e)
                {
                    Main.Log($"{depth} caught - {e}");
                }

                try
                {
                    foreach (var child in node.GetFieldNodes())
                    {
                        newTodo.Add(child);
                    }
                }
                catch (Exception e)
                {
                    Main.Log($"{depth} caught - {e}");
                }
            }

            if (visitCount % 1000 == 0)
            {
                yield return null;
            }
        }

        if (newTodo.Count > 0 && depth < Main.Settings.MaxSearchDepth)
        {
            yield return Search(searchText, newTodo, depth + 1, visitCount, sequenceNumber, updator, resultRoot);
        }
        else
        {
            Stop();
        }
    }
}
