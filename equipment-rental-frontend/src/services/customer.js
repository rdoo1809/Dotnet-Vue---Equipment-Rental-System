import api from './api'

export const customerService = {
  async getAll() {
    const response = await api.get('/customer')
    return response.data
  },

  async getById(id) {
    const response = await api.get(`/customer/${id}`)
    return response.data
  },

  async create(customer) {
    const response = await api.post('/customer', customer)
    return response.data
  },

  async update(id, customer) {
    const response = await api.put(`/customer/${id}`, customer)
    return response.data
  },

  async delete(id) {
    const response = await api.delete(`/customer/${id}`)
    return response.data
  },

}
