import api from './api'

export const equipmentService = {
  async getAll() {
    const response = await api.get('/equipment')
    return response.data
  },

  async getById(id) {
    const response = await api.get(`/equipment/${id}`)
    return response.data
  },

  async getAvailable() {
    const response = await api.get('/equipment/available')
    return response.data
  },

  async getRented() {
    const response = await api.get('/equipment/rented')
    return response.data
  },

  async create(equipment) {
    const response = await api.post('/equipment', equipment)
    return response.data
  },

  async update(id, equipment) {
    const response = await api.put(`/equipment/${id}`, equipment)
    return response.data
  },

  async delete(id) {
    const response = await api.delete(`/equipment/${id}`)
    return response.data
  }
}
