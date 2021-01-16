<?php 

class PatchFile {
	public $path;
	public $md5;
	public $size;
}

function getPatchFilesInFolder($dir, $basePath, &$results = array()) {
    $files = scandir($dir);

    foreach ($files as $key => $value) {
        $path = realpath($dir . DIRECTORY_SEPARATOR . $value);
        if (!is_dir($path)) {
	    $patchFile = new PatchFile();
	    $patchFile->path = substr($path, strlen($basePath) + 1);
        $patchFile->md5 = strtoupper(md5_file($path));
	    $patchFile->size = filesize($path);
            $results[] = $patchFile;
        } else if ($value != "." && $value != "..") {
            getPatchFilesInFolder($path, $basePath, $results);
        }
    }

    return $results;
}

function updateAndGetVersion($autoUpdateFile) {
	$currentValue = (int)file_get_contents($autoUpdateFile);
	$currentValue++;
	file_put_contents($autoUpdateFile, $currentValue);
	return $currentValue;
}

function writeIndexFile($indexFile, $files) {
	$bufferToWrite = "";
	foreach($files as $file) {
		$bufferToWrite .= $file->path . "|" . $file->md5 . "|" . $file->size . "\n";
	}
	file_put_contents($indexFile, $bufferToWrite);
}
