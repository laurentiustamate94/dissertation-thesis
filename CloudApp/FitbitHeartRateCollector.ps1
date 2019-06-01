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