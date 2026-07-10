# D2 ERD风格

##推荐标头```d2
vars: {
  d2-config: {
    layout-engine: elk
    theme-id: 300
  }
}
```
##表节点```d2
Clients: {
  shape: sql_table
  Id: uuid {constraint: primary_key}
  Name: varchar(200)
  Status: enum
}
```
# #的关系```d2
Offers.ClientId -> Clients.Id: "N:1"
```
# #风格```d2
classes: {
  join_table: {
    style.stroke-dash: 4
  }
  technical: {
    style.opacity: 0.55
  }
  optional_relation: {
    style.stroke-dash: 3
  }
}
```
