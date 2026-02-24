# tree-stop

```bash
apt update
apt upgrade -y
apt install -y curl wget socat cron
curl https://get.acme.sh | sh -s email=youremail@gmail.com
/root/.acme.sh/acme.sh --set-default-ca --server letsencrypt

ufw allow 80/tcp

bash <(curl -Ls https://raw.githubusercontent.com/mhsanaei/3x-ui/master/install.sh)
```
Save everything. And:

```bash
ufw allow xxx/tcp
```