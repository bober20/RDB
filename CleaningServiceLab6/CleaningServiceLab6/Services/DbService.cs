using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleaningServiceLab6.Models;
using Entities;

namespace CleaningServiceLab6.Services
{
    public class DbService
    {
        private readonly string _connectionString;

        public DbService(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionString").Value;
        }

        public async Task<List<Service>> GetAllServices()
        {
            var services = new List<Service>();

            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                await using (var cmd = new NpgsqlCommand("SELECT * FROM services", conn))
                {
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var service = new Service
                            {
                                Id = (int)reader["service_id"],
                                Name = (string)reader["name"],
                                Price = (int)reader["price"],
                                Area = (int)reader["area"],
                                ExtraInfo = (string)reader["extra_info"]
                            };

                            services.Add(service);
                        }
                    }
                }
            }

            return services;
        }


        public async Task<List<Order>> GetAllOrders(int userId)
        {
            var orders = new List<Order>();

            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                await using (var cmd = new NpgsqlCommand(
                                 $"select * from orders where orders.client_id = (select client_id from clients where user_id={userId}) or orders.employee_id = (select employee_id from employees where user_id={userId}); ",
                                 conn))
                {
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var order = new Order
                            {
                                Id = (int)reader["order_id"],
                                ClientId = reader["client_id"] != DBNull.Value ? (int)reader["client_id"] : 0,
                                EmployeeId = reader["employee_id"] != DBNull.Value ? (int)reader["employee_id"] : 0,
                                BonusId = reader["bonus_id"] != DBNull.Value ? (int)reader["bonus_id"] : 0,
                                ClientAddress = (string)reader["client_address"],
                                OrderDate = Convert.ToDateTime(reader["order_date"]),
                            };

                            order.Services = await GetOrderServices(order.Id);

                            orders.Add(order);
                        }
                    }
                }

                foreach (var order in orders)
                {
                    order.OrderSum = await GetOrderSum(order.Id);
                }
            }

            return orders;
        }

        public async Task DeleteOrder(int orderId)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                await using (var cmd = new NpgsqlCommand(
                                 $"delete from orders where order_id = {orderId}",
                                 conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task<List<Service>> GetOrderServices(int order_id)
        {
            var services = new List<Service>();
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                await using (var cmd = new NpgsqlCommand(
                                 $"select * from services_orders join services on services.service_id = services_orders.service_id where order_id = {order_id};",
                                 conn))
                {
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var service = new Service
                            {
                                Id = (int)reader["service_id"],
                                Price = (int)reader["price"],
                                Name = (string)reader["name"],
                                Area = (int)reader["area"],
                                ExtraInfo = (string)reader["extra_info"]
                            };

                            services.Add(service);
                        }
                    }
                }

                return services;
            }
        }

        public async Task<int> GetOrderSum(int orderId)
        {
            int orderSum = 0;

            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                await using (var cmd = new NpgsqlCommand($"SELECT * from get_order_sum({orderId});", conn))
                {
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (reader["get_order_sum"] != DBNull.Value)
                            {
                                orderSum = Convert.ToInt32(reader["get_order_sum"]);
                            }
                        }
                    }
                }
            }

            return orderSum;
        }

        public async Task<User> AuthenticateUser(LoginViewModel loginViewModel)
        {
            User user = null;

            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                await using (var cmd = new NpgsqlCommand(
                                 $"SELECT user_id, is_staff, is_admin from users where email='{loginViewModel.Email}' and password='{loginViewModel.Password}'",
                                 conn))
                {
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            user = new User
                            {
                                UserId = reader["user_id"] != DBNull.Value ? (int)reader["user_id"] : 0,
                                IsAdmin = (bool)reader["is_admin"],
                                IsStaff = (bool)reader["is_staff"]
                            };
                        }
                    }
                }
            }

            return user;
        }

        public async Task<bool> RegisterUser(RegistrationViewModel registrationViewModel)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"INSERT INTO users (first_name, last_name, email, password, is_staff, is_admin) VALUES ('{registrationViewModel.FirstName}', '{registrationViewModel.LastName}', '{registrationViewModel.Email}', '{registrationViewModel.Password}', {registrationViewModel.IsStaff}, {registrationViewModel.IsAdmin})",
                           conn))
                {
                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public async Task<List<User>> GetAllUsers()
        {
            var users = new List<User>();

            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand($"SELECT * from users", conn))
                {
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var user = new User
                            {
                                UserId = Convert.ToInt32(reader["user_id"]),
                                FirstName = (string)reader["first_name"],
                                LastName = (string)reader["last_name"],
                                Email = (string)reader["email"],
                                Password = (string)reader["password"],
                                IsStaff = (bool)reader["is_staff"],
                                IsAdmin = (bool)reader["is_admin"]
                            };

                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }

        public async Task UpdateUser(int userId, bool action, string role)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"UPDATE users SET {(role == "staff" ? "is_staff=" + action : "")} {(role == "admin" ? "is_admin=" + action : "")} WHERE user_id={userId}",
                           conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Question>> GetAllQuestions()
        {
            var questions = new List<Question>();

            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"SELECT * from questions",
                           conn))
                {
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var question = new Question
                            {
                                Id = Convert.ToInt32(reader["question_id"]),
                                AnswerId = reader["answer_id"] as int?,
                                Content = (string)reader["content"],
                                CreationDate = Convert.ToDateTime(reader["creation_date"])
                            };

                            questions.Add(question);
                        }
                    }
                }
            }

            return questions;
        }

        public async Task AddPosition(Question question)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"INSERT INTO questions (content) VALUES ('{question.Content}')",
                           conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task AddAnswer(Answer answer)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"INSERT INTO answers (content, employee_id) VALUES ('{answer.Content}', {answer.EmployeeId}); UPDATE questions SET answer_id = (SELECT MAX(answer_id) FROM answers) WHERE question_id = {answer.QuestionId}",
                           conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> GetEmployeeId(int userId)
        {
            int employeeId = 0;

            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"SELECT employee_id from employees where user_id={userId}",
                           conn))
                {
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            employeeId = Convert.ToInt32(reader["employee_id"]);
                        }
                    }
                }
            }

            return employeeId;
        }

        public async Task<int> GetClientId(int userId)
        {
            int clientId = -1;

            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"SELECT client_id FROM clients WHERE user_id={userId}",
                           conn))
                {
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            clientId = Convert.ToInt32(reader["client_id"]);
                        }
                    }
                }
            }

            return clientId;
        }

        public async Task<List<Review>> GetAllReviews()
        {
            var reviews = new List<Review>();

            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"SELECT * FROM review",
                           conn))
                {
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var review = new Review
                            {
                                Id = Convert.ToInt32(reader["review_id"]),
                                ClientId = Convert.ToInt32(reader["client_id"]),
                                Rate = Convert.ToInt16(reader["rate"]),
                                ReviewContent = reader["review_content"] as string,
                                ReviewDate = Convert.ToDateTime(reader["review_date"])
                            };

                            reviews.Add(review);
                        }
                    }
                }
            }

            return reviews;
        }

        public async Task AddReview(Review review)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"INSERT INTO review (client_id, rate, review_content) VALUES ({review.ClientId}, {review.Rate}, '{review.ReviewContent}')",
                           conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<EmployeePosition>> GetAllEmployeesPositions()
        {
            var employeePositions = new List<EmployeePosition>();

            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"SELECT * FROM employee_positions",
                           conn))
                {
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var position = new EmployeePosition
                            {
                                Id = Convert.ToInt32(reader["position_id"]),
                                PositionName = (string)reader["position_name"],
                                Salary = Convert.ToInt32(reader["salary"])
                            };

                            employeePositions.Add(position);
                        }
                    }
                }
            }

            return employeePositions;
        }

        public async Task<List<Vacancy>> GetAllVacancies()
        {
            var vacancies = new List<Vacancy>();

            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"SELECT * FROM vacancies",
                           conn))
                {
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var vacancy = new Vacancy
                            {
                                Id = Convert.ToInt32(reader["vacancy_id"]),
                                PositionId = Convert.ToInt32(reader["position_id"]),
                                VacancyNumber = Convert.ToInt32(reader["vacancy_number"]),
                                VacancyDescription = reader["vacancy_description"] as string
                            };

                            vacancies.Add(vacancy);
                        }
                    }
                }
            }

            return vacancies;
        }

        public async Task AddPosition(EmployeePosition position)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"INSERT INTO employee_positions (position_name, salary) VALUES ('{position.PositionName}', {position.Salary})",
                           conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeletePosition(int id)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"DELETE FROM employee_positions WHERE position_id = {id}",
                           conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateVacancyNumber(int vacancyId, int vacancyNumber)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"UPDATE vacancies SET vacancy_number={vacancyNumber} WHERE vacancy_id = {vacancyId}",
                           conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateVacancyDescription(int vacancyId, string vacancyDescription)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"UPDATE vacancies SET vacancy_description='{vacancyDescription}' WHERE vacancy_id = {vacancyId}",
                           conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task CreateService(Service service)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"INSERT INTO services (name, price, area, extra_info) VALUES ('{service.Name}', {service.Price}, {service.Area}, '{service.ExtraInfo}')",
                           conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteUser(int userId)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"DELETE FROM users WHERE user_id = {userId}",
                           conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task CreateOrder(Order order)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"INSERT INTO orders (client_id, employee_id, client_address, order_date) VALUES ({order.ClientId}, {order.EmployeeId}, '{order.ClientAddress}', '{order.OrderDate:yyyy-MM-dd HH:mm:ss}')",
                           conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task AddServiceToOrder(int orderId, int serviceId)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"INSERT INTO services_orders (service_id, order_id) VALUES ({serviceId}, {orderId})",
                           conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task RemoveServiceFromOrder(int orderId, int serviceId)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"DELETE FROM services_orders WHERE services_orders_id= (SELECT services_orders_id  FROM services_orders  WHERE service_id = {serviceId} AND order_id = {orderId} LIMIT 1)",
                           conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task<List<Employee>> GetAllEmployees()
        {
            var employees = new List<Employee>();

            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                await using (var cmd = new NpgsqlCommand(
                                 "select first_name, last_name, employee_id from users inner join (select employee_id, user_id from employees where position_id = (select position_id from employee_positions where position_name = 'Cleaner' )) as tempor on users.user_id = tempor.user_id;",
                                 conn))
                {
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var employee = new Employee
                            {
                                LastName = (string)reader["last_name"],
                                FirstName = (string)reader["first_name"],
                                Id = (int)reader["employee_id"],
                            };
                            employees.Add(employee);
                        }
                    }
                }
            }

            return employees;
        }
        
        public async Task<List<Bonus>> GetAllBonuses()
        {
            var bonuses = new List<Bonus>();

            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                await using (var cmd = new NpgsqlCommand(
                                 "SELECT bonus_id, discount_percentage FROM bonuses;", conn))
                {
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var bonus = new Bonus
                            {
                                Id = (int)reader["bonus_id"],
                                DiscountPercentage = (int)reader["discount_percentage"]
                            };

                            bonuses.Add(bonus);
                        }
                    }
                }
            }

            return bonuses;
        }
        
        public async Task CreateEmployee(EmployeeViewModel employee)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(
                           $"INSERT INTO users (first_name, last_name, email, password, is_admin) VALUES ('{employee.FirstName}', '{employee.LastName}', '{employee.Email}', '{employee.Password}', {employee.IsAdmin}); " +
                           $"INSERT INTO employees (user_id, position_id) VALUES ((SELECT user_id FROM users WHERE email='{employee.Email}'), {employee.PositionId});",
                           conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Answer>> GetAllAnswers()
        {
            var answers = new List<Answer>();
            
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                await using (var cmd = new NpgsqlCommand(
                                 $"select * from answers; ",
                                 conn))
                {
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var answer = new Answer()
                            {
                                Id = reader["answer_id"] != DBNull.Value ? (int)reader["answer_id"] : 0,
                                Content = (string)reader["content"]
                            };

                            answers.Add(answer);
                        }
                    }
                }

                return answers;
            }
        }
    }
}