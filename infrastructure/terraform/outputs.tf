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