# Proxy

Install any VPN on VPS as you like.
VPS with VPN it's a VPS2.
Remember the IP and port.

VPS1 -- proxy

VPS2 -- with VPN

On VPS1:

```bash
VPS2="123.123.123.123"
PORT="443"

# Включить маршрутизацию
sudo sysctl -w net.ipv4.ip_forward=1

# Сохранить маршрутизацию
echo 'net.ipv4.ip_forward = 1' | sudo tee /etc/sysctl.d/99-ipforward.conf >/dev/null

# DNAT
sudo iptables -t nat -A PREROUTING -p tcp --dport "$PORT" -j DNAT --to-destination "$VPS2:$PORT"

# MASQUERADE
sudo iptables -t nat -A POSTROUTING -p tcp -d "$VPS2" --dport "$PORT" -j MASQUERADE

# Разрешить новый трафик к VPS2:443
sudo iptables -A FORWARD -p tcp -d "$VPS2" --dport "$PORT" -m conntrack --ctstate NEW -j ACCEPT

# Разрешить ответный трафик
sudo iptables -A FORWARD -m conntrack --ctstate ESTABLISHED,RELATED -j ACCEPT

# Enable persistent rules
sudo apt install -y iptables-persistent
sudo netfilter-persistent save
```

При добавлении новых портов:

```bash
VPS2="123.123.123.123"
PORT="55444"

# NAT for 55444/udp
sudo iptables -t nat -A PREROUTING  -p udp --dport "$PORT" -j DNAT --to-destination "$VPS2:$PORT"
sudo iptables -t nat -A POSTROUTING -p udp -d "$VPS2" --dport "$PORT" -j MASQUERADE

# FORWARD for 55444/udp
sudo iptables -A FORWARD -p udp -d "$VPS2" --dport "$PORT" -m conntrack --ctstate NEW -j ACCEPT
```

save:

```bash
sudo netfilter-persistent save
sudo netfilter-persistent reload
```

check:

```bash
iptables -L -n -v --line-numbers
iptables -t nat -L -n -v --line-numbers
```

delete:

```bash
iptables -D FORWARD rule_num
iptables -t nat -D PREROUTING rule_num
iptables -t nat -D POSTROUTING rule_num
```