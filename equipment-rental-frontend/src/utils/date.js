export const formatDate = (dateString) => {
  if (!dateString) return ''
  return new Date(dateString).toLocaleDateString()
}

export const formatDateTime = (dateString) => {
  if (!dateString) return ''
  return new Date(dateString).toLocaleString()
}

export const getDaysDifference = (startDate, endDate) => {
  const start = new Date(startDate)
  const end = new Date(endDate)
  const diffTime = Math.abs(end - start)
  return Math.ceil(diffTime / (1000 * 60 * 60 * 24))
}
