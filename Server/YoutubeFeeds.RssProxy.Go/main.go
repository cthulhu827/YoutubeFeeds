package main

import (
	"io"
	"log"
	"net/http"
	"net/url"
)

func getRssHandler(w http.ResponseWriter, r *http.Request) {
	targetURL := r.URL.Query().Get("url")
	if targetURL == "" {
		http.Error(w, "URL parameter required", http.StatusBadRequest)
		return
	}

	if _, err := url.ParseRequestURI(targetURL); err != nil {
		http.Error(w, "Invalid URL format", http.StatusBadRequest)
		return
	}

	log.Printf("Fetching RSS from: %s", targetURL)

	client := &http.Client{}
	resp, err := client.Get(targetURL)
	if err != nil {
		log.Printf("Failed to fetch RSS: %v", err)
		http.Error(w, "Failed to fetch content: "+err.Error(), http.StatusInternalServerError)
		return
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		log.Printf("Non-OK status: %d", resp.StatusCode)
		http.Error(w, "Failed to fetch content", resp.StatusCode)
		return
	}

	w.Header().Set("Content-Type", "application/xml; charset=utf-8")
	_, err = io.Copy(w, resp.Body)
	if err != nil {
		log.Printf("Failed to write response: %v", err)
	}
}

func main() {
	http.HandleFunc("/api/get-rss", getRssHandler)
	
	log.Println("RSS Proxy server starting on :5231")
	if err := http.ListenAndServe(":5231", nil); err != nil {
		log.Fatalf("Server failed: %v", err)
	}
}
