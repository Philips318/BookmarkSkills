Python 3的代码示例

-使用Python 3的内置[`ipaddress`包](https://docs.python.org/3/library/ipaddress.html)，将`strict=True`传递给可用的构造函数。
-注意IPv4和IPv6地址解析-它们对专业网络工程师来说是不一样的。使用最强的type/class。
—一个子网可以包含一个主机：IPv6使用`/128`， IPv4使用`/32`。

IP地址和子网解析

—使用[`ipaddress`中的便利工厂功能]（https://docs.python.org/3/library/ipaddress.html#convenience-factory-functions）。    The following `ipaddress.ip_address(textAddress)` examples parse text into `IPv4Address` and `IPv6Address` objects, respectively:

    ```python
    ipaddress.ip_address('192.168.0.1')
    ipaddress.ip_address('2001:db8::')
    ```

    The following `ipaddress.ip_network(address, strict=True)` example parses a subnet string and returns an `IPv4Network` or `IPv6Network` object, failing on invalid input:

    ```python
    ipaddress.ip_network('192.168.0.0/28', strict=True)
    ```

    The following strict-mode call fails (correctly) with `ValueError: 192.168.0.1/30 has host bits set`. Ask the user to fix such errors; do not guess corrections:

    ```python
    ipaddress.ip_network('192.168.0.1/30', strict=True)
    ```
—使用严格形式解析器[`ipaddress.ip_network(address, strict=True)`]（https://docs.python.org/3/library/ipaddress.html#ipaddress.ip_network）。

IP子网字典

使用Python字典来跟踪子网及其相关的地理位置属性。`IPv4Network`、`IPv6Network`、`IPv4Address`和`IPv6Address`都是可哈希的，可以用作字典键。

##检测非公共IP范围SKILL.md引用`is_private`来检测非公共范围。使用网络属性：```python
import ipaddress

def is_non_public(network):
    """Check if a network is non-public (private, loopback, link-local, multicast, or reserved).
    
    Note: In Python < 3.11, is_private may incorrectly flag some ranges
    (e.g., 100.64.0.0/10 CGNAT space). Use is_global as the primary check
    when available, with fallbacks for edge cases.
    """
    addr = network.network_address
    return (
        network.is_private
        or network.is_loopback
        or network.is_link_local
        or network.is_multicast
        or network.is_reserved
        or not network.is_global  # catches most non-routable space
    )
```
**`100.64.0.0/10`（运营商级NAT）范围返回`is_private=True`，但在旧的Python版本中返回`is_global=False`。由于CGNAT空间不是全局可路由的，因此将其标记为非公共对于RFC 8805目的是正确的。

ISO 3166-1国家代码验证

从[assets/iso3166-1.json]（../assets/iso3166-1.json）中读取有效的ISO 2字母国家代码，特别是`alpha_2`属性：```python
import json

with open('assets/iso3166-1.json') as f:
    data = json.load(f)
    valid_countries = {c['alpha_2'] for c in data['3166-1']}
```
ISO 3166-2区域代码验证

从[assets/iso3166-2.json]（../assets/iso3166-2.json）读取有效的区域代码，特别是`code`属性。顶级键是`3166-2`（匹配iso3166-1模式）：```python
import json

with open('assets/iso3166-2.json') as f:
    data = json.load(f)
    valid_regions = {r['code'] for r in data['3166-2']}
```
