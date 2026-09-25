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