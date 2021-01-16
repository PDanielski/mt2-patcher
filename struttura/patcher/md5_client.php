<?php 
require_once("common.php");

$sourceFolder = "source";
$indexFile = "metaFiles/Index";
$autoUpdateFile = "source/AutoUpdate";

$postPatchVersion = updateAndGetVersion($autoUpdateFile);

$files = getPatchFilesInFolder($sourceFolder, realpath($sourceFolder));

writeIndexFile($indexFile, $files);

echo "Patch client versione ". $postPatchVersion . " rilasciata";

