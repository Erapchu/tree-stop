# Proxy

Install any VPN on VPS as you like.
VPS with VPN it's a VPS2.
Remember the IP and port.

VPS1 -- proxy

VPS2 -- with VPN

On VPS1:

```bash
VPS2_IP="123.123.123.123"
AWG_PORT="54321"   # example, replace with AmneziaWG port for example
#SSH_PORT="22"

sudo sysctl -w net.ipv4.ip_forward=1
#echo 'net.ipv4.ip_forward = 1' | sudo tee /etc/sysctl.d/99-ipforward.conf >/dev/null
#sudo sysctl -p /etc/sysctl.d/99-ipforward.conf

# NAT: inbound 54321 -> VPS2:54321
sudo iptables -t nat -A PREROUTING  -p udp --dport "$AWG_PORT" -j DNAT --to-destination "$VPS2_IP:$AWG_PORT"
sudo iptables -t nat -A POSTROUTING -p udp -d "$VPS2_IP" --dport "$AWG_PORT" -j MASQUERADE

# FORWARD only to VPS2:54321 + established
sudo iptables -A FORWARD -m conntrack --ctstate ESTABLISHED,RELATED -j ACCEPT
sudo iptables -A FORWARD -p udp -d "$VPS2_IP" --dport "$AWG_PORT" -m conntrack --ctstate NEW -j ACCEPT

# INPUT: lo, established, SSH
# sudo iptables -A INPUT -i lo -j ACCEPT
# sudo iptables -A INPUT -m conntrack --ctstate ESTABLISHED,RELATED -j ACCEPT
# sudo iptables -A INPUT -p tcp --dport "$SSH_PORT" -j ACCEPT

# after checking close everything else
# sudo iptables -P INPUT DROP
# sudo iptables -P FORWARD DROP
# sudo iptables -P OUTPUT ACCEPT

# Enable persistent rules
sudo apt install -y iptables-persistent
sudo netfilter-persistent save
```

```bash
VPS2_IP="123.123.123.123"
PORT_443="443"

# NAT for 443/tcp
sudo iptables -t nat -A PREROUTING  -p tcp --dport "$PORT_443" -j DNAT --to-destination "$VPS2_IP:$PORT_443"
sudo iptables -t nat -A POSTROUTING -p tcp -d "$VPS2_IP" --dport "$PORT_443" -j MASQUERADE

# FORWARD for 443/tcp
sudo iptables -A FORWARD -p tcp -d "$VPS2_IP" --dport "$PORT_443" -m conntrack --ctstate NEW -j ACCEPT
```

save:

```bash
sudo netfilter-persistent save
sudo netfilter-persistent reload
```

check:

```bash
iptables -L -n -v --line-numbers
```