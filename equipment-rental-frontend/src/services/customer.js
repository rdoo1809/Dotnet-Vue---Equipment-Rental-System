import api from './api'

export const customerService = {
  async getAll() {
    const response = await api.get('/customers')
    return response.data
  },

  async getById(id) {
    const response = await api.get(`/customers/${id}`)
    return response.data
  },

  async create(customer) {
    const response = await api.post('/customers', customer)
    return response.data
  },

  async update(id, customer) {
    const response = await api.put(`/customers/${id}`, customer)
    return response.data
  },

  async delete(id) {
    const response = await api.delete(`/customers/${id}`)
    return response.data
  },

  async getRentals(id) {
    const response = await api.get(`/customers/${id}/rentals`)
    return response.data
  },

  async getActiveRental(id) {
    const response = await api.get(`/customers/${id}/active-rental`)
    return response.data
  }
}
