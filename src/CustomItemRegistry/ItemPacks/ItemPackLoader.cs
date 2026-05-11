using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;

namespace ValheimCustomItemRegistry
{
    internal static class ItemPackLoader
    {
        private static readonly IItemPackParser[] Parsers =
        {
            new YamlItemPackParser(),
            new JsonItemPackParser()
        };

        public static string DefaultPackDirectory => Path.Combine(Paths.ConfigPath, "CustomItemRegistry", "packs");

        public static ItemPackParserStatus GetParserStatus()
        {
            return ItemPackParserStatus.Create();
        }

        public static ItemPackLoadResult LoadDefault(ItemPackLoadOptions options)
        {
            return LoadDirectory(DefaultPackDirectory, options);
        }

        public static ItemPackLoadResult LoadDirectory(string directory, ItemPackLoadOptions options)
        {
            options = options ?? new ItemPackLoadOptions();
            ItemPackLoadResult result = new ItemPackLoadResult { SourcePath = directory };

            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
            {
                return result;
            }

            SearchOption searchOption = options.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            IEnumerable<string> files = Directory.EnumerateFiles(directory, "*.*", searchOption)
                .Where(IsSupportedExtension)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase);

            foreach (string file in files)
            {
                result.Add(LoadFile(file, options));
            }

            LogSummary(result, options);
            return result;
        }

        public static ItemPackLoadResult LoadFile(string filePath, ItemPackLoadOptions options)
        {
            options = options ?? new ItemPackLoadOptions();
            ItemPackLoadResult result = new ItemPackLoadResult { SourcePath = filePath };
            ItemPackFileResult fileResult = new ItemPackFileResult { FilePath = filePath };
            result.Files.Add(fileResult);

            IItemPackParser parser = Parsers.FirstOrDefault(candidate => candidate.CanParse(filePath));
            if (parser == null)
            {
                fileResult.Skipped = true;
                fileResult.SkipReason = "Unsupported item-pack file extension";
                return result;
            }

            fileResult.Format = parser.FormatName;
            if (!parser.IsAvailable)
            {
                fileResult.Skipped = true;
                fileResult.SkipReason = parser.MissingDependencyMessage;
                CustomItemRegistry.LogWarning($"{parser.MissingDependencyMessage} Skipping '{filePath}'.");
                return result;
            }

            try
            {
                ItemPackDto pack = parser.Parse(File.ReadAllText(filePath));
                fileResult.PackName = pack?.Name;
                fileResult.PackVersion = pack?.Version;

                if (pack?.Items == null || pack.Items.Count == 0)
                {
                    fileResult.Errors.Add(new ItemPackRegistrationError(filePath, null, "Item pack contains no items"));
                    return result;
                }

                foreach (ItemPackItemDto item in pack.Items)
                {
                    RegisterItem(filePath, options, fileResult, item);
                }
            }
            catch (Exception exception)
            {
                string message = Unwrap(exception).Message;
                fileResult.Errors.Add(new ItemPackRegistrationError(filePath, null, message, exception));
                CustomItemRegistry.LogWarning($"Could not load item pack '{filePath}': {message}");
            }

            return result;
        }

        private static void RegisterItem(string filePath, ItemPackLoadOptions options, ItemPackFileResult fileResult, ItemPackItemDto item)
        {
            CustomItemDefinition definition = null;
            try
            {
                definition = ItemPackMapper.ToDefinition(item, filePath, options);
                if (!options.RegisterItems)
                {
                    return;
                }

                ItemRegistrationResult itemResult;
                CustomItemRegistry.TryRegisterItem(definition, out itemResult);
                fileResult.Items.Add(itemResult);

                if (!itemResult.Success)
                {
                    fileResult.Errors.Add(new ItemPackRegistrationError(filePath, definition.ItemName, itemResult.ErrorMessage, itemResult.Exception));
                }
            }
            catch (Exception exception)
            {
                string itemName = definition?.ItemName ?? item?.ItemName;
                string message = Unwrap(exception).Message;
                fileResult.Errors.Add(new ItemPackRegistrationError(filePath, itemName, message, exception));
                CustomItemRegistry.LogWarning($"Could not register item-pack item '{itemName ?? "<unknown>"}' from '{filePath}': {message}");
            }
        }

        private static bool IsSupportedExtension(string filePath)
        {
            return Parsers.Any(parser => parser.CanParse(filePath));
        }

        private static Exception Unwrap(Exception exception)
        {
            return exception is System.Reflection.TargetInvocationException target && target.InnerException != null
                ? target.InnerException
                : exception;
        }

        private static void LogSummary(ItemPackLoadResult result, ItemPackLoadOptions options)
        {
            if (!options.LogSummary || result.Files.Count == 0)
            {
                return;
            }

            CustomItemRegistry.LogInfo(
                $"Loaded CIR item packs from '{result.SourcePath}'. Files={result.Files.Count}; registered={result.RegisteredItemCount}; skipped={result.SkippedFileCount}; errors={result.FailedItemCount}.");
        }
    }
}
