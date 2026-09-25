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