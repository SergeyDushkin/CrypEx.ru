<?php
	$to_cur = strip_tags(htmlspecialchars($_GET['to_cur']));

	//get MOEX exchange rate
	$url = 'http://iss.moex.com/iss/engines/currency/markets/selt/boards/CNGD/securities/USD000000TOD.json';

	$json = file_get_contents($url);
	$obj = json_decode($json);
	$usdrur = $obj->marketdata->data[0][8];

	//get Bitfinex exchange rate
	$url = 'https://api.bitfinex.com/v1/ticker/' . $to_cur . 'usd';

	$json = file_get_contents($url);
	$obj = json_decode($json);
	$curtousd = $obj->last_price;

	header('Content-type: application/json');
	echo $usdrur*$curtousd;

?>