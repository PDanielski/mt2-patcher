<?php 
require_once("common.php");

$sourceFolder = "sourceUpdater";
$indexFile = "metaFiles/UpdaterIndex";

$files = getPatchFilesInFolder($sourceFolder, realpath($sourceFolder));

writeIndexFile($indexFile, $files);

echo "Patch patcher rilasciata";

