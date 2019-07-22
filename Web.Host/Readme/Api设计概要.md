## 1 定义
> [service]
* 由[*子系统]-[子模块]-[孙模块]组成，其中模块是非必须的
* 完全小写
 
> [controller]
* 控制器名，一般代表某项小功能，比如[books] [users] [roomstucts]等，
* 都是是复数形式
* 完全小写

> [id]
* 表示某项的一个唯一标识
* 比如用户名或用户id

## 2 路由规则
### 2.1 内部接口
> internal/[service]/[controller]/[id]

* internal/rke-admin/users
* internal/rke-admin/books/id001


### 2.2 对外接口
> api/[service]/[controller]/[id]

* api/rke-admin/users
* api/rke-admin/users/id002
* api/rke-app/roomstructs
* api/rke-device/门口机/roomstructs
* api/rke-device/室内机/roomstructs/id003
* api/rke-device-log/门口机/unlockedlogs
* api/rke-保安/users


## 3 请求方式
> GET

用于获取资源的请求，其最大特点是多次获取一个资源不会对资源有改变的风险，
客户端可以在一个业务请求里进行多次重试GET操作。

> POST

等同于add，用于给资源服务器添加资源

> PUT

等同于覆盖性update，将传入的整条资源数据覆盖更新到已有的记录上

> PATCH

等同于选择性update，将传入感兴趣的一个或多个属性更新到已有记录对应的字段上

> DELETE

删除用途，对于单一记录其路由一般设计为DELETE api/[service]/[controller]/[id]

## 4 响应约定
### 4.1 http状态码标记请求异常
* 401 未授权的请求
* 404 请求api不存在
* 405 不正确的请求方式
* 500 服务器内部异常

### 4.2 使用统一的模型描述业务状态与数据
* code定义业务操作状态码
* message为对应的提示语
* data为成功时对应的接口数据模型
* http状态码为200到299

```
{
  "code": 0,
  "message": "成功",
  "data": {
    "name": "laojiu",
    "age": 99     
  }
}
```


## 5 查询条件与分页模型

### 5.1 查询条件
> ps 此功能未确定好，最终和前端确定是使用query还是cookie

前端多个查询条件使用query参数传递到相应的api

* api/rke-admin/users?name=laojiu
* api/rke-admin/users?name=laojiu&nickName=九
* api/rke-admin/users?name=laojiu&nickName=九&userType=3

后端应当有能力适应前端传入不确定数量的查询字段的能力，
并且有权利使用合适的比较符号和忽略传入的query参数名，
比如以上查询后端可以解释为
* name like '%laojiu%'
* name like '%laojiu%' // 强制忽略了nickName
* name like '%laojiu%' and nickName like '%九%' and userType = 3

### 5.2 分页模型
前端使用pageIndex和pageSize两个query参数定位分页，pageIndex为索引，从0开始。
后端返回如数据的data对应以下分页模型:

```
{
  "pageIndex": 0,
  "pageSize": 10,
  "totalCount": 999,
  "dataArray": [
    {},
    {}
  ]
}
```