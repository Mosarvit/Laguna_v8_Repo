<?php
$servername = "mosar.heliohost.org";
$username = "mosar_1";
$password = "Fahrenheit";
$dbname = "mosar_flashcards_db";
// Create connection
$con = new mysqli($servername, $username, $password, $dbname);

if (!$con)
{
	die('Could not connect: ' . mysql_error());
} 
  
$result = mysqli_query($con, "SELECT * FROM flashcards");

while($row = mysqli_fetch_assoc($result))
{
	$output[]=$row;
}
  
print(json_encode($output));
mysqli_close($con);
?>