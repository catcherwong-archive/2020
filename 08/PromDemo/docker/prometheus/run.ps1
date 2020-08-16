$base = Split-Path -Parent $MyInvocation.MyCommand.Definition
$prometheusyml = Join-Path $base prometheus.yml
$fileconfig = Join-Path $base "config"

write-host $prometheusyml
write-host $fileconfig

docker run `
    -p 9099:9090 `
    -v ${prometheusyml}:/etc/prometheus/prometheus.yml `
    -v ${fileconfig}:/etc/prometheus/fileconfig `
    prom/prometheus:v2.20.1