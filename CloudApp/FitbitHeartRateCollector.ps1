[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls -bor [Net.SecurityProtocolType]::Tls11 -bor [Net.SecurityProtocolType]::Tls12
$ProgressPreference="SilentlyContinue"
try
{ 
    $Res = Invoke-WebRequest __CLOUD_ENDPOINT__/Fitbit/PersistData -UseBasicParsing 
    $Res.StatusCode 
}
catch
{
    $ErrorMessage = $_.Exception.Message
    $ErrorMessage
    Exit 1
}