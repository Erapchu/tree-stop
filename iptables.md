Добавить:

```bash
iptables -A INPUT -p tcp --dport 11653 -j ACCEPT
```

Проверить:

```bash
iptables -L INPUT -n -v --line-numbers
```

Удалить:

```bash
iptables -L INPUT -n --line-numbers
iptables -D INPUT <номер>

#Либо
iptables -D INPUT -p tcp --dport 11653 -j ACCEPT
```
