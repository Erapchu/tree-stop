## Configure xray

```bash
bash <(curl -Ls https://raw.githubusercontent.com/mhsanaei/3x-ui/master/install.sh)
```

### Check cron jobs (should be ~/.acme.sh)

```bash
crontab -l
```

And set self-signed certificate.

On duckdns.org create new domain for you.

```bash
# if have problems with certificates
~/.acme.sh/acme.sh --remove -d yourdomain --ecc
rm -rf ~/.acme.sh/yourdomain_ecc
rm -rf ~/.acme.sh/yourdomain
rm -rf /root/cert/yourdomain

iptables -A INPUT -p tcp --dport 11653 -j ACCEPT # TCP
iptables -A INPUT -p udp --dport 8955 -j ACCEPT # UDP
iptables -L INPUT -n -v --line-numbers # check everything
```