output "vpc_id" {
  description = "ID of the application VPC."
  value       = aws_vpc.main.id
}

output "public_subnet_ids" {
  description = "IDs of the public subnets."
  value = [
    for subnet in aws_subnet.public : subnet.id
  ]
}

output "app_subnet_ids" {
  description = "IDs of the application subnets."
  value = [
    for subnet in aws_subnet.app : subnet.id
  ]
}

output "database_subnet_ids" {
  description = "IDs of the database subnets."
  value = [
    for subnet in aws_subnet.database : subnet.id
  ]
}

output "availability_zones" {
  description = "Availability zones used by the application."
  value       = local.availability_zones
}

output "alb_security_group_id" {
  description = "Security group ID for the application load balancer."
  value       = aws_security_group.alb.id
}

output "ecs_security_group_id" {
  description = "Security group ID for ECS tasks."
  value       = aws_security_group.ecs.id
}

output "rds_security_group_id" {
  description = "Security group ID for RDS PostgreSQL."
  value       = aws_security_group.rds.id
}

output "redis_security_group_id" {
  description = "Security group ID for Redis."
  value       = aws_security_group.redis.id
}

output "kafka_security_group_id" {
  description = "Security group ID for Kafka."
  value       = aws_security_group.kafka.id
}

output "rds_endpoint" {
  description = "RDS PostgreSQL endpoint."
  value       = aws_db_instance.postgres.address
}

output "rds_port" {
  description = "RDS PostgreSQL port."
  value       = aws_db_instance.postgres.port
}

output "rds_database_name" {
  description = "RDS PostgreSQL database name."
  value       = aws_db_instance.postgres.db_name
}

output "redis_primary_endpoint" {
  description = "Primary endpoint for the ElastiCache Valkey replication group."
  value       = aws_elasticache_replication_group.redis.primary_endpoint_address
}

output "redis_reader_endpoint" {
  description = "Reader endpoint for the ElastiCache Valkey replication group."
  value       = aws_elasticache_replication_group.redis.reader_endpoint_address
}

output "redis_port" {
  description = "Port used by ElastiCache Valkey."
  value       = aws_elasticache_replication_group.redis.port
}

output "database_secret_arn" {
  description = "ARN of the database secret."
  value       = aws_secretsmanager_secret.database.arn
}

output "redis_secret_arn" {
  description = "ARN of the Redis/Valkey secret."
  value       = aws_secretsmanager_secret.redis.arn
}

output "jwt_secret_arn" {
  description = "ARN of the JWT secret."
  value       = aws_secretsmanager_secret.jwt.arn
}

output "ecs_execution_role_arn" {
  description = "ARN of the ECS task execution role."
  value       = aws_iam_role.ecs_execution.arn
}

output "ecs_task_role_arn" {
  description = "ARN of the ECS application task role."
  value       = aws_iam_role.ecs_task.arn
}

output "github_actions_ecr_role_arn" {
  description = "ARN of the GitHub Actions ECR role."
  value       = aws_iam_role.github_actions_ecr.arn
}

output "ecs_cluster_id" {
  description = "ECS cluster ID."
  value       = aws_ecs_cluster.main.id
}

output "ecs_cluster_name" {
  description = "ECS cluster name."
  value       = aws_ecs_cluster.main.name
}

output "ecs_service_name" {
  description = "ECS service name."
  value       = aws_ecs_service.api.name
}

output "ecs_task_definition_arn" {
  description = "ECS task definition ARN."
  value       = aws_ecs_task_definition.api.arn
}

output "ecs_log_group_name" {
  description = "CloudWatch log group used by ECS."
  value       = aws_cloudwatch_log_group.ecs.name
}

output "alb_dns_name" {
  description = "DNS name of the application load balancer."
  value       = aws_lb.api.dns_name
}

output "alb_arn" {
  description = "ARN of the application load balancer."
  value       = aws_lb.api.arn
}

output "alb_target_group_arn" {
  description = "ARN of the ECS API target group."
  value       = aws_lb_target_group.api.arn
}