# RedisWebApiSample

Implement distributed caching using redis on .NET Web API (.net 7)

## steps:
1. run redis `server docker run -d --name redis-server -p 6379:6379 redis`
2. (Optional) open interactive terminal on redis container `docker exec -it redis-server sh`
3. (Optional) try redis cli `redis-cli` , `SET mykey "hello redis"`, `GET mykey`


Reference: https://www.youtube.com/watch?v=6HZVu3kGOrg&ab_channel=MohamadLawand
