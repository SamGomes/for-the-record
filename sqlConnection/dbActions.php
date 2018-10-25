<?php

header("Access-Control-Allow-Credentials: true");
header("Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
header("Access-Control-Allow-Methods: GET, POST, OPTIONS");
header("Access-Control-Allow-Origin: *");


function update_form_data($success, $table, $arrFields, $arrValues, $extraParams)
{
    $sql = 'UPDATE '.$table.' SET ';

	for ($i = 0; $i < count($arrFields); $i++) {
		$sql = $sql . ' ' . $arrFields[$i]. ' = "'. $arrValues[$i] . '"';
		if($i < (count($arrFields) - 1)){
			$sql = $sql . ',';
		}
	}
	$sql = $sql ." ". $extraParams;

    error_log($sql);
    return mysqli_query($success,$sql);
}

function insert_form_data($success, $table, $arrFields, $arrValues)
{
    $sql = 'INSERT INTO '.$table.' ('.implode(',', $arrFields).') VALUES (\''.implode('\', \'',  $arrValues).'\')';
    error_log($sql);
    return mysqli_query($success,$sql);
}
function select_form_data($success, $table, $arrFields, $extraParams)
{
    $sql = 'SELECT '.implode(',', $arrFields).' FROM '.$table.' '.$extraParams;
    error_log($sql);
    $query = mysqli_query($success, $sql);
    if($query === FALSE) { 
	   die(mysqli_error());
	}

	$result = [];

	while($row = mysqli_fetch_assoc($query)){
		foreach($arrFields as $arrField){
			echo $row[$arrField]; //return results to outside with echo
		}
	}
}


if (isset($_POST["dbAction"])) {
	$dbAction = $_POST["dbAction"];
	$databaseName = $_POST["databaseName"];
	$tableName = $_POST["tableName"];
	$arrFields = $_POST["arrFields"];
	$arrValues = $_POST["arrValues"];
	$extraParams = $_POST["extraParams"];


	$connection = new mysqli('localhost','admin','admin', $databaseName);

	// if(!$connection->connect_error){
	// 	echo "1";
	// 	exit();
	// }
	switch ($dbAction) {
	case "INSERT":
		# code...
 		insert_form_data($connection,$tableName,$arrFields,$arrValues);
		break;
	case 'SELECT':
		# code...
		select_form_data($connection,$tableName,$arrFields,$extraParams);
		break;
	case 'UPDATE':
		# code...
		update_form_data($connection,$tableName,$arrFields, $arrValues, $extraParams);
		break;
	default:
		# code...
		break;
	}
}
?>