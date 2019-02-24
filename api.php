<?php
require 'SLIM/Slim/Slim.php';
\Slim\Slim::registerAutoLoader();

header('Access-Control-Allow-Origin: *');
header('Content-type: application/json');

$app = new \Slim\Slim();

$app->get('/ranking/:data(/:skip(/:take))', function($data, $skip = null, $take = null) {
	$db = new SQLite3('FIS.sqlite');
	$json = array();
	$sql = "SELECT r.jumper, j.name, j.surname, j.nation, COUNT(r.fispoints) AS starts,
			(SELECT CASE WHEN COUNT(r.fispoints) < 5 THEN ROUND(AVG(r.fispoints) * (15 - COUNT(r.fispoints)) / 10, 2)
				ELSE ROUND((SELECT AVG(fispoints) FROM (SELECT * FROM results JOIN competitions ON results.competitionid = competitions.id 
				WHERE jumper = r.jumper AND competitions.date BETWEEN DATE('$data', '-1 year') AND '$data' ORDER BY fispoints LIMIT 5)), 2)
				END) AS Punkty_FIS, (SELECT c.date FROM results LEFT JOIN competitions c ON results.competitionid = c.id WHERE results.jumper = r.jumper ORDER BY c.date DESC LIMIT 1) AS last
			FROM results r JOIN jumpers j ON r.jumper = j.fiscode JOIN competitions c ON r.competitionid = c.id 
			WHERE c.date BETWEEN DATE('$data', '-1 year') AND '$data' GROUP BY r.jumper ORDER BY Punkty_FIS";
	if ($skip != null)
		$sql .= " LIMIT $skip, $take";
	$query = $db->query($sql);
	while ($result = $query->fetchArray(SQLITE3_NUM)) {
		$json[] = array("FIScode" => $result[0], "name" => $result[1], "surname" => $result[2], "nation" => substr($result[3], 0, 3),
			"starts" => $result[4], "FISpoints" => $result[5], "last" => $result[6]);
	}
	$db->exec("DELETE FROM tempRanking");
	$db->exec("INSERT INTO tempRanking " . $sql);
	$db->close();
	echo json_encode($json);
});

$app->get('/quotas', function() {
	$db = new SQLite3('FIS.sqlite');
	$db->exec("DELETE FROM quotas");
	$results = array();
	$query = $db->query("SELECT DISTINCT nation FROM tempRanking WHERE FISpoints < 75");
	while ($result = $query->fetchArray(SQLITE3_NUM)) {
		$wc = $db->querySingle("SELECT CASE WHEN COUNT(nation) > 6 THEN 6 ELSE COUNT(nation) END FROM tempRanking WHERE FISpoints < 30 AND nation='$result[0]'");
		$coc = $db->querySingle("SELECT CASE WHEN COUNT(nation) > 7 THEN 7 ELSE COUNT(nation) END FROM tempRanking WHERE FISpoints BETWEEN 25 AND 50 and nation='$result[0]'");
		$fc = $db->querySingle("SELECT CASE WHEN COUNT(nation) > 8 THEN 8 ELSE COUNT(nation) END FROM tempRanking WHERE FISpoints BETWEEN 45 AND 75 and nation='$result[0]'");
		$db->exec("INSERT INTO quotas VALUES ('$result[0]', '$wc', '$coc', '$fc')");
		//$results[] = array("nation" => $result[0], "WC" => $wc, "COC" => $coc, "FC" => $fc);
	}
	$query2 = $db->query("SELECT * FROM quotas ORDER BY WC DESC, COC DESC, FC DESC, nation ASC");
	while ($res = $query2->fetchArray(SQLITE3_NUM)) {
		$results[] = array("nation" => $res[0], "WC" => $res[1], "COC" => $res[2], "FC" => $res[3]);
	}
	$db->close();
	echo json_encode($results);
});

$app->get('/competition/:id', function($id) {
	$db = new SQLite3('FIS.sqlite');	
	$results = array();
	$info = array();
	$max = $db->querySingle("SELECT MAX(sum) FROM results WHERE competitionID = $id");
	$min = $db->querySingle("SELECT MIN(sum) FROM results WHERE competitionID = $id");
	$delta = ($max - $min) / 99;
	$query = $db->query("SELECT * FROM competitions WHERE id = $id");
	while ($result = $query->fetchArray(SQLITE3_NUM)) {
		$info[] = array("id" => $result[0], "name" => $result[1], "location" => $result[2], "date" => $result[3], "HS" => $result[4],
		"penalty" => $result[5], "max" => $max, "min" => $min, "delta" => $delta);
	}
	$sql = "SELECT r.jumper, j.name, j.surname, j.nation, r.sum, r.fispoints 
		FROM results r LEFT JOIN jumpers j ON r.jumper = j.fiscode WHERE r.competitionid = $id";
	$query = $db->query($sql);
	while ($result = $query->fetchArray(SQLITE3_NUM)) {
		$fis1 = round(100 - (($max - $result[4]) / $delta));
		$results[] = array("FIScode" => $result[0], "name" => $result[1], "surname" => $result[2], "nation" => substr($result[3], 0, 3),
			"sum" => $result[4], "FISpoints" => $result[5], "FIS1" => $fis1);
	}
	$json = array("info" => $info, "results" => $results);
	$db->close();
	echo json_encode($json);
});

$app->get('/jumper/:id', function($id) {
	$db = new SQLite3('FIS.sqlite');
	$json = array();
	$results = array();
	$info = array();
	$query = $db->query("SELECT * FROM jumpers WHERE FIScode = $id");
	while ($result = $query->fetchArray(SQLITE3_NUM)) {
		$info[] = array("FIScode" => $result[0], "name" => $result[1], "surname" => $result[2], "dateOfBirth" => $result[3],
		"nation" => substr($result[4], 0, 3));
	}
	$sql = "SELECT r.jumper, r.sum, r.fispoints, r.competitionid, c.location, c.date, c.name, c.id FROM results r LEFT JOIN competitions c ON r.competitionid = c.id
			WHERE r.jumper = $id ORDER BY c.date DESC, r.competitionid DESC";
	$query = $db->query($sql);
	while ($result = $query->fetchArray(SQLITE3_NUM)) {
		$sql2 = "SELECT (SELECT CASE WHEN COUNT(r.fispoints) < 5 THEN ROUND(AVG(r.fispoints) * (15 - COUNT(r.fispoints)) / 10, 2) ELSE ROUND(
			(SELECT AVG(fispoints) FROM (SELECT * FROM results JOIN competitions ON results.competitionid = competitions.id 
			WHERE results.jumper = $result[0] AND results.competitionid <= $result[3] AND competitions.date BETWEEN DATE('$result[5]', '-1 year')
			AND '$result[5]' ORDER BY fispoints LIMIT 5)), 2) END) AS Punkty_FIS
			FROM results r LEFT JOIN competitions c ON r.competitionid = c.id WHERE r.jumper = $result[0] AND r.competitionid <= $result[3]
				AND c.date BETWEEN DATE('$result[5]', '-1 year') AND '$result[5]'";
		$fis = $db->querySingle($sql2);
		$results[] = array("FIScode" => $result[0], "sum" => $result[1], "FISpoints" => $result[2], "location" => $result[4],	
			"date" => $result[5], "name" => $result[6], "ID" => $result[7], "ranking" => $fis);
	}
	$db->close();
	$json = array("jumper" => $info, "results" => $results);
	echo json_encode($json);
});

$app->get('/jumper/:id/:day', function($id, $day){
	$db = new SQLite3('FIS.sqlite');
	$sql = "SELECT (SELECT CASE WHEN COUNT(r.fispoints) < 5 THEN ROUND(AVG(r.fispoints) * (15 - COUNT(r.fispoints)) / 10, 2) ELSE ROUND(
			(SELECT AVG(fispoints) FROM (SELECT * FROM results JOIN competitions ON results.competitionid = competitions.id 
			WHERE results.jumper = $id AND competitions.date BETWEEN DATE('$day', '-1 year') AND '$day' ORDER BY fispoints LIMIT 5)), 2) END) AS Punkty_FIS
			FROM results r LEFT JOIN competitions c ON r.competitionid = c.id WHERE r.jumper = $id AND c.date BETWEEN DATE('$day', '-1 year') AND '$day'";
	$result = $db->querySingle($sql);
	echo json_encode($result);
});

$app->get('/jumpers', function() {
	$db = new SQLite3('FIS.sqlite');
	$json = array();
	$sql = "SELECT * FROM jumpers";
	$query = $db->query($sql);
	while ($row = $query->fetchArray(SQLITE3_NUM)) {
		$json[] = array("FIScode" => $row[0], "name" => $row[1], "surname" => $row[2], "dateOfBirth" => $row[3], "nation" => substr($row[4], 0, 3));
	}
	$db->close();
	echo json_encode($json);
});

$app->get('/competitions', function() {
	$db = new SQLite3('FIS.sqlite');	
	$json = array();
	$query = $db->query("SELECT * FROM competitions ORDER BY date DESC");
	while ($result = $query->fetchArray(SQLITE3_NUM)) {
		$json[] = array("id" => $result[0], "name" => $result[1], "location" => $result[2], "date" => $result[3], "HS" => $result[4], "penalty" => $result[5]);
	}
	$db->close();
	echo json_encode($json);
});

$app->run();

?>
