# RDB
### Сорока Дарья Федоровна
### группа 253503
### выбранная область: клининговая компания

docker run -d \
  --name cleaning-service-db \
  --network cleaning-service \
  --volume cleaning-service:/var/lib/postgresql/data \
  -e POSTGRES_PASSWORD=mysecretpassword \
  postgres

docker exec -it cleaning-service-db psql -U postgres


docker run -d \
  --name cleaning-service-db \
  --network cleaning-service \
  --volume cleaning-service:/var/lib/postgresql/data \
  -e POSTGRES_PASSWORD=mysecretpassword \
  -p 5432:5432 \
  postgres



