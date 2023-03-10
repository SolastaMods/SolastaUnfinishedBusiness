using System;
using System.Collections.Generic;
using System.Linq;

namespace SolastaUnfinishedBusiness.Api.ModKit
{
    namespace BlueprintExplorer
    {
        public interface ISearchable
        {
            Dictionary<string, Func<string>>
                Providers { get; } // named functions to extract different text out of the target

            Dictionary<string, MatchResult> Matches { get; set; } // place to store search results
        }

        public static class MatchHelpers
        {
            public static bool HasMatches(this ISearchable searchable, float scoreThreshold = 10)
            {
                return searchable.Matches == null ||
                       searchable.Matches.Any(m => m.Value.IsMatch && m.Value.Score >= scoreThreshold);
            }
        }

        public class MatchResult
        {
            public readonly MatchQuery Context;
            private readonly string key;
            private readonly List<Span> spans = new();

            private readonly ISearchable target;
            public readonly string Text;
            private int bestRun;
            public float Bonus;
            public int MatchedCharacters;
            public float Penalty;
            private int singleRuns;
            public float TargetRatio;
            public int TotalMatched;

            public MatchResult(ISearchable target, string key, string text, MatchQuery context)
            {
                this.target = target;
                this.key = key;
                Text = text;
                Context = context;
            }

            public bool IsMatch => TotalMatched > 0;
            public float MatchRatio => TotalMatched / (float)Context.SearchText.Length;
            public int GoodRuns => spans.Count(x => x.Length > 2); //> 2);

            public float Score => (TargetRatio * MatchRatio * 1.0f) + (bestRun * 4) + (GoodRuns * 2) - Penalty + Bonus;

            public void AddSpan(Span span)
            {
                spans.Add(span);

                //update some stats that get used for scoring
                if (span.Length > bestRun)
                {
                    bestRun = span.Length;
                }

                if (span.Length == 1)
                {
                    singleRuns++;
                }

                TotalMatched += span.Length;
            }

            public struct Span
            {
                public readonly UInt16 From;
                public UInt16 Length;

                public Span(int start, int length = -1)
                {
                    From = (ushort)start;
                    Length = (ushort)length;
                }

                public void End(int end)
                {
                    Length = (UInt16)(end - From);
                }
            }
        }

        public abstract class MatchQuery
        {
            private readonly Dictionary<string, string> restrictedSearchTexts; // restricted to certain provider keys
            public readonly string SearchText; // general search text

            protected MatchQuery(string queryText)
            {
                var unrestricted = new List<string>();
                restrictedSearchTexts = new Dictionary<string, string>();
                var terms = queryText.Split(' ');
                foreach (var term in terms)
                {
                    if (term.Contains(':'))
                    {
                        var pair = term.Split(':');
                        restrictedSearchTexts[pair[0]] = pair[1];
                    }
                    else
                    {
                        unrestricted.Add(term);
                    }
                }

                SearchText = string.Join(" ", unrestricted);
            }

            private MatchResult Match(string searchText, ISearchable searchable, string key, string text)
            {
                var result = new MatchResult(searchable, key, text, this);
                var index = text.IndexOf(searchText, StringComparison.CurrentCulture);
                if (index < 0)
                {
                    return result;
                }

                var span = new MatchResult.Span(index, searchText.Length);
                result.AddSpan(span);
                result.TargetRatio = result.TotalMatched / (float)text.Length;

                return result;
            }

            private MatchResult FuzzyMatch(ISearchable searchable, string key, string text)
            {
                var result = new MatchResult(searchable, key, text, this);

                var searchTextIndex = 0;
                var searchText = result.Context.SearchText;
                var target = result.Text;

                // find a common prefix if any, so n:cat h:cats grace is better than n:cat h:blah cats grace
                var targetIndex = target.IndexOf(searchText[searchTextIndex]);
                if (targetIndex == 0)
                {
                    result.Bonus = 2.0f;
                }

                // penalise matches that don't have a common prefix, while increasing searchTextIndex and targetIndex to the first match, so:
                // n:bOb  h:Hello World
                //    ^         ^
                while (targetIndex == -1 && searchTextIndex < searchText.Length)
                {
                    if (searchTextIndex == 0)
                    {
                        result.Penalty = 2;
                    }
                    else
                    {
                        result.Penalty += result.Penalty * .5f;
                    }

                    targetIndex = target.IndexOf(searchText[searchTextIndex]);
                    searchTextIndex++;
                }

                // continue to match the next searchTextIndex greedily in target
                while (searchTextIndex < searchText.Length)
                {
                    // find the next point in target that matches searchIndex:
                    // n:bOb h:helloworldBob
                    //     ^             ^
                    targetIndex = target.IndexOf(searchText[searchTextIndex], targetIndex);
                    if (targetIndex == -1)
                    {
                        break;
                    }

                    //continue matching while both are in sync
                    var span = new MatchResult.Span(targetIndex);
                    while (targetIndex < target.Length && searchTextIndex < searchText.Length &&
                           searchText[searchTextIndex] == target[targetIndex])
                    {
                        //if this span is rooted at the start of the word give a bonus because start is most importatn
                        if (span.From == 0 && searchTextIndex > 0)
                        {
                            result.Bonus += result.Bonus;
                        }

                        searchTextIndex++;
                        targetIndex++;
                    }

                    //record the end of the span
                    span.End(targetIndex);
                    result.AddSpan(span);
                }

                result.TargetRatio = result.TotalMatched / (float)target.Length;
                return result;
            }

            private ISearchable Evaluate(ISearchable searchable)
            {
                if (SearchText?.Length > 0 || restrictedSearchTexts.Count > 0)
                {
                    searchable.Matches = new Dictionary<string, MatchResult>();
                    foreach (var provider in searchable.Providers)
                    {
                        var key = provider.Key;
                        var text = provider.Value();
                        var foundRestricted = false;
                        foreach (var entry in restrictedSearchTexts.Where(entry => key.StartsWith(entry.Key)))
                        {
                            searchable.Matches[key] = Match(entry.Value, searchable, key, text);
                            foundRestricted = true;
                            break;
                        }

                        if (!foundRestricted && SearchText?.Length > 0)
                        {
                            searchable.Matches[key] = FuzzyMatch(searchable, key, text);
                        }
                    }
                }
                else
                {
                    searchable.Matches = null;
                }

                return searchable;
            }

            public void UpdateSearchResults(IEnumerable<ISearchable> searchables)
            {
                foreach (var searchable in searchables)
                {
                    Evaluate(searchable);
                }
            }
        }

#if false
    public static class FuzzyMatcher {
        public static IEnumerable<T> FuzzyMatch<T>(this IEnumerable<(T, string)> input, string needle, float scoreThreshold
 = 10) {
            var result = new MatchQuery<T>(needle.ToLower());
            return input.Select(i => result.Match(i.Item2, i.Item1)).Where(match => match.Score > scoreThreshold).OrderByDescending(match => match.Score).Select(m => m.Handle);
        }
        /// <summary>
        /// Fuzzy Match all items in input against the needle
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">input items</param>
        /// <param name="haystack">function to get the 'string' key of an input item to match against</param>
        /// <param name="needle">value to match against</param>
        /// <param name="scoreThreshold">discard all results under this score (default: 10)</param>
        /// <returns>An IEnumerable<out T> that contains elements from the input sequence that score above the threshold, sorted by score</out></returns>
        public static IEnumerable<T> FuzzyMatch<T>(this IEnumerable<T> input, Func<T, string> haystack, string needle, float scoreThreshold
 = 10) {
            return input.Select(i => (i, haystack(i))).FuzzyMatch(needle, scoreThreshold);
        }


        public class ExampleType {
            int Foo;
            string Bar;

            public string Name => $"{Bar}.{Foo}";
        }

        public static void Example() {
            //Assume some input list (or enumerable)
            List<ExampleType> inputList = new();

            //Get an enumerable of all the matches (above a score threshold, default = 10)
            var matches = inputList.FuzzyMatch(type => type.Name, "string_to_search");

            //Get top 20 results
            var top20 = matches.Take(20).ToList();
        }

    }
#endif
    }
}
