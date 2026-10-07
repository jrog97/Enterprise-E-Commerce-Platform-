resource "aws_elasticache_subnet_group" "redis" {
  name = "${var.project_name}-redis-subnet-group"

  subnet_ids = [
    for subnet in aws_subnet.database : subnet.id
  ]

  tags = {
    Name = "${var.project_name}-redis-subnet-group"
  }
}

resource "aws_elasticache_replication_group" "redis" {
  replication_group_id = "${var.project_name}-redis"

  description = "Valkey cache for the Enterprise E-Commerce application"

  engine         = "valkey"
  engine_version = "8.0"

  node_type = "cache.t4g.micro"

  num_cache_clusters = 2

  automatic_failover_enabled = true
  multi_az_enabled           = true

  port = 6379

  subnet_group_name = aws_elasticache_subnet_group.redis.name

  security_group_ids = [
    aws_security_group.redis.id
  ]

  at_rest_encryption_enabled = true
  transit_encryption_enabled = true

  auth_token = var.redis_auth_token

  auto_minor_version_upgrade = true

  snapshot_retention_limit = 7
  snapshot_window          = "03:00-04:00"

  maintenance_window = "sun:04:00-sun:05:00"

  tags = {
    Name = "${var.project_name}-redis"
  }
}