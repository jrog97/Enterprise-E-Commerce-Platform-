variable "aws_region" {
  description = "AWS region where the infrastructure will be deployed."
  type        = string
  default     = "us-east-2"
}

variable "project_name" {
  description = "Name of the project."
  type        = string
  default     = "enterprise-ecommerce"
}

variable "environment" {
  description = "Deployment environment."
  type        = string
  default     = "production"
}

variable "vpc_cidr" {
  description = "CIDR block for the application VPC."
  type        = string
  default     = "172.31.0.0/16"
}

variable "database_username" {
  description = "PostgreSQL master username."
  type        = string
  sensitive   = true
}

variable "database_password" {
  description = "PostgreSQL master password."
  type        = string
  sensitive   = true
}

variable "redis_auth_token" {
  description = "Authentication token for ElastiCache Valkey."
  type        = string
  sensitive   = true
}

data "aws_kms_key" "kafka" {
  key_id = "alias/aws/kafka"
}

variable "jwt_issuer" {
  description = "JWT issuer."
  type        = string
  sensitive   = true
}

variable "jwt_audience" {
  description = "JWT audience."
  type        = string
  sensitive   = true
}

variable "jwt_key" {
  description = "JWT signing key."
  type        = string
  sensitive   = true
}