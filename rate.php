<?php
require_once 'library/Requests.php';

$request = Requests::get('https://www.bitfinex.com/');

var_dump($request->status_code);
// int(200)

var_dump($request->headers['content-type']);
// string(31) "application/json; charset=utf-8"

var_dump($request->body);
// string(26891) "[...]"

/*
$url = 'https://www.bitfinex.com/';
$ch = curl_init($url);
curl_setopt($ch, CURLOPT_POST, 1);
curl_setopt($ch, CURLOPT_POSTFIELDS, $xml);
curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
$response = curl_exec($ch);
echo $response;
curl_close($ch);
*/

$r = new HttpRequest('https://www.bitfinex.com/', HttpRequest::METH_GET);

try {
    $r->send();
    if ($r->getResponseCode() == 200) {
        echo $r->getResponseBody();
    }
}
catch (HttpException $ex) {
    echo $ex;
}
?>