<?php

function insert_form_data($success, $table, $arrFields, $arrValues)
{
    $sql = 'INSERT INTO '.$table.' ('.implode(',', $arrFields).') VALUES (\''.implode('\', \'',  $arrValues).'\')';
    error_log($sql);
    return mysqli_query($success,$sql);
}



if (isset($_POST["dbAction"])) {
	$dbAction = $_POST["dbAction"];
	$databaseName = $_POST["databaseName"];
	$tableName = $_POST["tableName"];
	$arrFields = $_POST["arrFields"];
	$arrValues = $_POST["arrValues"];


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
		//select_form_data($tableName,$arrValues);
		break;
	default:
		# code...
		break;
	}
}
?>