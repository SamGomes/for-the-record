<?php

function insert_form_data($success, $table, $arrFields, $arrValues)
{
    $sql = 'INSERT INTO '.$table.' ('.implode(',', $arrFields).') VALUES (\''.implode('\', \'',  $arrValues).'\')';
    error_log($sql);
    return mysqli_query($success,$sql);
}
function select_form_data($success, $table, $arrFields, $extraParams)
{
    $sql = 'SELECT '.implode(',', $arrFields).' FROM '.$table.' '.$extraParams;
    $query = mysqli_query($success, $sql);
    if($query === FALSE) { 
	   die(mysqli_error());
	}

	$result = [];

	while($row = mysqli_fetch_assoc($query)){
		foreach($arrFields as $arrField){
			echo $row[$arrField];
		}
	}
    //echo $result;
}


if (isset($_POST["dbAction"])) {
	$dbAction = $_POST["dbAction"];
	$databaseName = $_POST["databaseName"];
	$tableName = $_POST["tableName"];
	$arrFields = $_POST["arrFields"];
	$arrValues = $_POST["arrValues"];
	$extraParams = $_POST["extraParams"];


	$connection = new mysqli('localhost','root','rootroot',$databaseName);

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
	default:
		# code...
		break;
	}
}
?>