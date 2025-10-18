import api from './api'

export const rentalService = {
  async getAll() {
    const response = await api.get('/rental')
    return response.data
  },

  async getById(id) {
    const response = await api.get(`/rental/${id}`)
    return response.data
  },

  async getActive() {
    const response = await api.get('/rental/active')
    return response.data
  },

  async getCompleted() {
    const response = await api.get('/rental/completed')
    return response.data
  },

  async getOverdue() {
    const response = await api.get('/rental/overdue')
    return response.data
  },

  async issue(rentalData) {
    const response = await api.post('/rental/issue', rentalData)
    return response.data
  },

  async return(rentalData) {
    const response = await api.post('/rental/return', rentalData)
    return response.data
  },

  async extend(id, extensionData) {
    const response = await api.put(`/rental/${id}`, extensionData)
    return response.data
  },

  async cancel(id) {
    const response = await api.delete(`/rental/${id}`)
    return response.data
  },

}
